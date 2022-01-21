﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using Naninovel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <inheritdoc cref="IScriptPlayer"/>
    [InitializeAtRuntime]
    public class ScriptPlayer : IStatefulService<SettingsStateMap>, IStatefulService<GlobalStateMap>, IStatefulService<GameStateMap>, IScriptPlayer
    {
        [Serializable]
        public class Settings
        {
            public PlayerSkipMode SkipMode = PlayerSkipMode.ReadOnly;
        }

        [Serializable]
        public class GlobalState
        {
            public PlayedScriptRegister PlayedScriptRegister = new PlayedScriptRegister();
        }

        [Serializable]
        public class GameState
        {
            public bool Playing;
            public bool ExecutedPlayedCommand;
            public bool WaitingForInput;
            public List<PlaybackSpot> GosubReturnSpots;
        }

        public event Action<Script> OnPlay;
        public event Action<Script> OnStop;
        public event Action<Command> OnCommandExecutionStart;
        public event Action<Command> OnCommandExecutionFinish;
        public event Action<bool> OnSkip;
        public event Action<bool> OnAutoPlay;
        public event Action<bool> OnWaitingForInput;

        public virtual ScriptPlayerConfiguration Configuration { get; }
        public virtual bool Playing => playRoutineCTS != null;
        public virtual bool SkipAllowed => GetSkipAllowed();
        public virtual bool SkipActive { get; private set; }
        public virtual bool AutoPlayActive { get; private set; }
        public virtual bool WaitingForInput { get; private set; }
        public virtual PlayerSkipMode SkipMode { get; set; }
        public virtual Script PlayedScript { get; private set; }
        public virtual Command PlayedCommand => Playlist?.GetCommandByIndex(PlayedIndex);
        public virtual PlaybackSpot PlaybackSpot => PlayedCommand?.PlaybackSpot ?? default;
        public virtual ScriptPlaylist Playlist { get; private set; }
        public virtual int PlayedIndex { get; private set; }
        public virtual Stack<PlaybackSpot> GosubReturnSpots { get; private set; }
        public virtual int PlayedCommandsCount => playedScriptRegister.CountPlayed();

        private readonly ResourceProviderConfiguration providerConfig;
        private readonly List<Func<Command, UniTask>> preExecutionTasks = new List<Func<Command, UniTask>>();
        private readonly List<Func<Command, UniTask>> postExecutionTasks = new List<Func<Command, UniTask>>();
        private readonly Queue<Func<UniTask>> onSynchronizeTasks = new Queue<Func<UniTask>>();
        private readonly IInputManager inputManager;
        private IScriptManager scriptManager;
        private IStateManager stateManager;
        private int executedCommandsCount = 0;
        private bool executedPlayedCommand;
        private PlayedScriptRegister playedScriptRegister;
        private CancellationTokenSource playRoutineCTS;
        private CancellationTokenSource commandExecutionCTS;
        private CancellationTokenSource synchronizationCTS;
        private UniTaskCompletionSource waitForWaitForInputDisabledTCS;
        private UniTaskCompletionSource synchronizeTCS;
        private IInputSampler continueInput, skipInput, autoPlayInput;

        public ScriptPlayer (ScriptPlayerConfiguration config, ResourceProviderConfiguration providerConfig, IInputManager inputManager)
        {
            Configuration = config;
            this.providerConfig = providerConfig;
            this.inputManager = inputManager;

            GosubReturnSpots = new Stack<PlaybackSpot>();
            playedScriptRegister = new PlayedScriptRegister();
            commandExecutionCTS = new CancellationTokenSource();
            synchronizationCTS = new CancellationTokenSource();
        }

        public virtual UniTask InitializeServiceAsync ()
        {
            scriptManager = Engine.GetService<IScriptManager>();
            stateManager = Engine.GetService<IStateManager>();
            
            continueInput = inputManager.GetContinue();
            skipInput = inputManager.GetSkip();
            autoPlayInput = inputManager.GetAutoPlay();

            if (continueInput != null)
            {
                continueInput.OnStart += DisableWaitingForInput;
                continueInput.OnStart += DisableSkip;
            }
            if (skipInput != null)
            {
                skipInput.OnStart += EnableSkip;
                skipInput.OnEnd += DisableSkip;
            }
            if (autoPlayInput != null)
                autoPlayInput.OnStart += ToggleAutoPlay;

            if (Configuration.ShowDebugOnInit)
                UI.DebugInfoGUI.Toggle();

            return UniTask.CompletedTask;
        }

        public virtual void ResetService ()
        {
            Stop();
            CancelCommands();
            // Playlist?.ReleaseResources(); performed in StateManager; 
            // here it could be invoked after the actors are already destroyed.
            Playlist = null;
            PlayedIndex = -1;
            PlayedScript = null;
            executedPlayedCommand = false;
            DisableWaitingForInput();
            DisableAutoPlay();
            DisableSkip();
        }

        public virtual void DestroyService ()
        {
            ResetService();

            commandExecutionCTS?.Dispose();
            synchronizationCTS?.Dispose();

            if (continueInput != null)
            {
                continueInput.OnStart -= DisableWaitingForInput;
                continueInput.OnStart -= DisableSkip;
            }
            if (skipInput != null)
            {
                skipInput.OnStart -= EnableSkip;
                skipInput.OnEnd -= DisableSkip;
            }
            if (autoPlayInput != null)
                autoPlayInput.OnStart -= ToggleAutoPlay;
        }

        public virtual void SaveServiceState (SettingsStateMap stateMap)
        {
            var settings = new Settings {
                SkipMode = SkipMode
            };
            stateMap.SetState(settings);
        }

        public virtual UniTask LoadServiceStateAsync (SettingsStateMap stateMap)
        {
            var settings = stateMap.GetState<Settings>() ?? new Settings();
            SkipMode = settings.SkipMode;
            return UniTask.CompletedTask;
        }

        public virtual void SaveServiceState (GlobalStateMap stateMap)
        {
            var globalState = new GlobalState {
                PlayedScriptRegister = playedScriptRegister
            };
            stateMap.SetState(globalState);
        }

        public virtual UniTask LoadServiceStateAsync (GlobalStateMap stateMap)
        {
            var state = stateMap.GetState<GlobalState>() ?? new GlobalState();
            playedScriptRegister = state.PlayedScriptRegister;
            return UniTask.CompletedTask;
        }

        public virtual void SaveServiceState (GameStateMap stateMap)
        {
            var gameState = new GameState {
                Playing = Playing,
                ExecutedPlayedCommand = executedPlayedCommand,
                WaitingForInput = WaitingForInput,
                GosubReturnSpots = GosubReturnSpots.Count > 0 ? GosubReturnSpots.Reverse().ToList() : null // Stack is reversed on enum.
            };
            stateMap.PlaybackSpot = PlaybackSpot;
            stateMap.SetState(gameState);
        }

        public virtual async UniTask LoadServiceStateAsync (GameStateMap stateMap)
        {
            var state = stateMap.GetState<GameState>();
            if (state is null)
            {
                ResetService();
                return;
            }

            // Force stop and cancel all running commands to prevent state mutation while loading other services.
            Stop(); CancelCommands();

            executedPlayedCommand = state.ExecutedPlayedCommand;

            if (state.Playing) // The playback is resumed (when necessary) after other services are loaded.
            {
                if (stateManager.RollbackInProgress) stateManager.OnRollbackFinished += PlayAfterRollback;
                else stateManager.OnGameLoadFinished += PlayAfterLoad;
            }
            
            if (state.GosubReturnSpots != null && state.GosubReturnSpots.Count > 0)
                GosubReturnSpots = new Stack<PlaybackSpot>(state.GosubReturnSpots);
            else GosubReturnSpots.Clear();

            if (!string.IsNullOrEmpty(stateMap.PlaybackSpot.ScriptName))
            {
                if (PlayedScript is null || !stateMap.PlaybackSpot.ScriptName.EqualsFast(PlayedScript.Name))
                {
                    PlayedScript = await scriptManager.LoadScriptAsync(stateMap.PlaybackSpot.ScriptName);
                    Playlist = new ScriptPlaylist(PlayedScript, scriptManager);
                    PlayedIndex = Playlist.IndexOf(stateMap.PlaybackSpot);
                    Debug.Assert(PlayedIndex >= 0, $"Failed to load script player state: `{stateMap.PlaybackSpot}` doesn't exist in the current playlist.");
                    var endIndex = providerConfig.ResourcePolicy == ResourcePolicy.Static ? Playlist.Count - 1 :
                        Mathf.Min(PlayedIndex + providerConfig.DynamicPolicySteps, Playlist.Count - 1);
                    await Playlist.PreloadResourcesAsync(PlayedIndex, endIndex);
                }
                else PlayedIndex = Playlist.IndexOf(stateMap.PlaybackSpot);
            }
            else
            {
                Playlist?.Clear();
                PlayedScript = null;
                PlayedIndex = 0;
            }

            void PlayAfterRollback ()
            {
                stateManager.OnRollbackFinished -= PlayAfterRollback;
                SetWaitingForInputEnabled(state.WaitingForInput);
                // Rollback snapshots are pushed before the currently played command is executed, so play it again.
                Play();
            }

            void PlayAfterLoad (GameSaveLoadArgs _)
            {
                stateManager.OnGameLoadFinished -= PlayAfterLoad;
                SetWaitingForInputEnabled(state.WaitingForInput);
                // Game could be saved before or after the currently played command is executed.
                if (executedPlayedCommand)
                {
                    if (SelectNextCommand()) Play();
                }
                else Play();
            }
        }

        public virtual void AddPreExecutionTask (Func<Command, UniTask> task) => preExecutionTasks.Insert(0, task);

        public virtual void RemovePreExecutionTask (Func<Command, UniTask> task) => preExecutionTasks.Remove(task);

        public virtual void AddPostExecutionTask (Func<Command, UniTask> task) => postExecutionTasks.Insert(0, task);

        public virtual void RemovePostExecutionTask (Func<Command, UniTask> task) => postExecutionTasks.Remove(task);

        public virtual void Play ()
        {
            if (PlayedScript is null || Playlist is null)
                throw new Exception("Failed to start script playback: the script is not assigned.");

            if (Playing) Stop();

            if (Playlist.IsIndexValid(PlayedIndex) || SelectNextCommand())
            {
                playRoutineCTS = new CancellationTokenSource();
                PlayRoutineAsync(playRoutineCTS.Token).Forget();
                OnPlay?.Invoke(PlayedScript);
            }
        }

        public virtual void Play (ScriptPlaylist playlist, int playlistIndex)
        {
            if (Playlist != playlist)
                Playlist?.ReleaseResources();
            
            Playlist = playlist;
            PlayedIndex = playlistIndex;
            Play();
        }

        public virtual void Play (Script script, int startLineIndex = 0, int startInlineIndex = 0)
        {
            PlayedScript = script;

            if (Playlist is null || Playlist.ScriptName != script.Name)
            {
                Playlist?.ReleaseResources();
                Playlist = new ScriptPlaylist(script, scriptManager);
            }

            if (startLineIndex > 0 || startInlineIndex > 0)
            {
                var startCommand = Playlist.GetCommandAfterLine(startLineIndex, startInlineIndex);
                if (startCommand is null) throw new Exception($"Script player failed to start: no commands found in script `{PlayedScript.Name}` at line #{startLineIndex}.{startInlineIndex}.");
                PlayedIndex = Playlist.IndexOf(startCommand);
            }
            else PlayedIndex = 0;

            Play();
        }

        public virtual async UniTask PreloadAndPlayAsync (string scriptName, int startLineIndex = 0, int startInlineIndex = 0, string label = null)
        {
            var script = await scriptManager.LoadScriptAsync(scriptName);
            if (script is null) throw new Exception($"Script player failed to start: script with name `{scriptName}` wasn't able to load.");

            if (!string.IsNullOrEmpty(label))
            {
                if (!script.LabelExists(label)) throw new Exception($"Failed navigating script playback to `{label}` label: label not found in `{script.Name}` script.");
                startLineIndex = script.GetLineIndexForLabel(label);
                startInlineIndex = 0;
            }

            var prevPlaylist = Playlist; // Release later to prevent re-loading resources used in both scripts.
            Playlist = new ScriptPlaylist(script, scriptManager);
            var startAction = Playlist.GetCommandAfterLine(startLineIndex, startInlineIndex);
            var startIndex = startAction != null ? Playlist.IndexOf(startAction) : 0;
            var endIndex = providerConfig.ResourcePolicy == ResourcePolicy.Static ? Playlist.Count - 1 :
                Mathf.Min(startIndex + providerConfig.DynamicPolicySteps, Playlist.Count - 1);
            await Playlist.PreloadResourcesAsync(startIndex, endIndex);
            prevPlaylist?.ReleaseResources();
            await Resources.UnloadUnusedAssets();

            Play(script, startLineIndex, startInlineIndex);
        }
        
        public async UniTask PlayTransientAsync (ScriptPlaylist playlist, CancellationToken cancellationToken = default)
        {
            foreach (var command in playlist)
            {
                if (cancellationToken.CancelASAP) return;
                if (!command.ShouldExecute) continue;
                if (command.ForceWait || PlayedCommand.Wait) 
                    await command.ExecuteAsync(cancellationToken);
                else command.ExecuteAsync(cancellationToken).Forget();
            }
        }

        public virtual void Stop ()
        {
            if (Playing)
            {
                playRoutineCTS.Cancel();
                playRoutineCTS.Dispose();
                playRoutineCTS = null;

                OnStop?.Invoke(PlayedScript);
            }

            DisableWaitingForInput();
        }

        public virtual async UniTask<bool> RewindAsync (int lineIndex)
        {
            if (PlayedCommand is null) throw new Exception("Script player failed to rewind: played command is not valid.");

            var targetCommand = Playlist.GetCommandAfterLine(lineIndex, 0);
            if (targetCommand is null) throw new Exception($"Script player failed to rewind: target line index ({lineIndex}) is not valid for `{PlayedScript.Name}` script.");

            var targetPlaylistIndex = Playlist.IndexOf(targetCommand);
            if (targetPlaylistIndex == PlayedIndex) return true;

            if (Playing) Stop();

            var wasWaitingInput = WaitingForInput;
            DisableAutoPlay();
            DisableSkip();
            DisableWaitingForInput();

            playRoutineCTS = new CancellationTokenSource();
            var cancellationToken = playRoutineCTS.Token;

            bool result;
            if (targetPlaylistIndex > PlayedIndex)
            {
                // In case were waiting input, the current command wasn't executed; execute it now.
                result = await FastForwardRoutineAsync(cancellationToken, targetPlaylistIndex, wasWaitingInput);
                Play();
            }
            else
            {
                var targetSpot = new PlaybackSpot(PlayedScript.Name, lineIndex, 0);
                result = await stateManager.RollbackAsync(s => s.PlaybackSpot == targetSpot);
            }

            return result;
        }

        public virtual void SetSkipEnabled (bool enable)
        {
            if (SkipActive == enable) return;
            if (enable && !SkipAllowed) return;

            SkipActive = enable;
            Time.timeScale = enable ? Configuration.SkipTimeScale : 1f;
            OnSkip?.Invoke(enable);

            if (enable && WaitingForInput) SetWaitingForInputEnabled(false);
        }

        public virtual void SetAutoPlayEnabled (bool enable)
        {
            if (AutoPlayActive == enable) return;
            AutoPlayActive = enable;
            OnAutoPlay?.Invoke(enable);

            if (enable && WaitingForInput) SetWaitingForInputEnabled(false);
        }

        public virtual void SetWaitingForInputEnabled (bool enable)
        {
            if (WaitingForInput == enable) return;

            if (SkipActive && enable || (!enable && (continueInput.Active || AutoPlayActive)))
                stateManager.PeekRollbackStack()?.AllowPlayerRollback();

            if (SkipActive && enable) return;

            WaitingForInput = enable;
            if (!enable)
            {
                waitForWaitForInputDisabledTCS?.TrySetResult();
                waitForWaitForInputDisabledTCS = null;
            }

            OnWaitingForInput?.Invoke(enable);
        }
        
        public async UniTask SynchronizeAndDoAsync (Func<UniTask> task)
        {
            onSynchronizeTasks.Enqueue(task);
            
            if (synchronizeTCS != null)
            {
                await synchronizeTCS.Task;
                return;
            }

            var clickThroughUI = Engine.GetService<IUIManager>().GetUI<UI.ClickThroughPanel>();
            if (clickThroughUI) 
                clickThroughUI.Show(false, null);
            
            synchronizationCTS.Cancel();
            synchronizeTCS = new UniTaskCompletionSource();

            await UniTask.WaitWhile(() => executedCommandsCount > 0);

            while (onSynchronizeTasks.Count > 0)
                await onSynchronizeTasks.Dequeue()();

            synchronizationCTS.Dispose();
            synchronizationCTS = new CancellationTokenSource();
            synchronizeTCS.TrySetResult();
            synchronizeTCS = null;
            
            if (clickThroughUI) 
                clickThroughUI.Hide();
        }

        /// <summary>
        /// In case synchronization is performed, will wait until it's completed;
        /// returns true in case provided token has requested ASAP cancellation.
        /// </summary>
        /// <remarks>This should be awaited after any async operation in the playback routine.</remarks>
        protected async UniTask<bool> WaitSynchronizeAsync (CancellationToken cancellationToken)
        {
            if (cancellationToken.CancelASAP) return true;
            if (synchronizeTCS != null)
                await synchronizeTCS.Task;
            return cancellationToken.CancelASAP;
        }
        
        private void EnableSkip () => SetSkipEnabled(true);
        private void DisableSkip () => SetSkipEnabled(false);
        private void EnableAutoPlay () => SetAutoPlayEnabled(true);
        private void DisableAutoPlay () => SetAutoPlayEnabled(false);
        private void ToggleAutoPlay () => SetAutoPlayEnabled(!AutoPlayActive);
        private void EnableWaitingForInput () => SetWaitingForInputEnabled(true);
        private void DisableWaitingForInput () => SetWaitingForInputEnabled(false);

        private bool GetSkipAllowed ()
        {
            if (SkipMode == PlayerSkipMode.Everything) return true;
            if (PlayedScript is null) return false;
            return playedScriptRegister.IsIndexPlayed(PlayedScript.Name, PlayedIndex);
        }

        private async UniTask WaitForWaitForInputDisabledAsync ()
        {
            if (waitForWaitForInputDisabledTCS is null)
                waitForWaitForInputDisabledTCS = new UniTaskCompletionSource();
            await waitForWaitForInputDisabledTCS.Task;
        }

        private async UniTask WaitForAutoPlayDelayAsync ()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Configuration.MinAutoPlayDelay));
            if (!AutoPlayActive) await WaitForWaitForInputDisabledAsync(); // In case auto play was disabled while waiting for delay.
        }

        private async UniTask ExecutePlayedCommandAsync (CancellationToken cancellationToken)
        {
            if (PlayedCommand is null || !PlayedCommand.ShouldExecute) return;

            OnCommandExecutionStart?.Invoke(PlayedCommand);

            playedScriptRegister.RegisterPlayedIndex(PlayedScript.Name, PlayedIndex);

            for (int i = preExecutionTasks.Count - 1; i >= 0; i--)
            {
                await preExecutionTasks[i](PlayedCommand);
                if (await WaitSynchronizeAsync(cancellationToken)) return;
            }
            
            if (await WaitSynchronizeAsync(cancellationToken)) return;

            var synchronizationToken = synchronizationCTS.Token;
            executedPlayedCommand = true;
            executedCommandsCount++;
            
            if (Configuration.CompleteOnContinue && continueInput != null && PlayedCommand.Wait && !PlayedCommand.ForceWait)
            {
                var syncAndInputCTS = CancellationTokenSource.CreateLinkedTokenSource(synchronizationToken, continueInput.GetInputStartCancellationToken());
                var executionToken = new CancellationToken(commandExecutionCTS.Token, syncAndInputCTS.Token);
                await PlayedCommand.ExecuteAsync(executionToken);
                syncAndInputCTS.Dispose();
                executedCommandsCount--;
            }
            else if (PlayedCommand.Wait || PlayedCommand.ForceWait)
            {
                var executionToken = new CancellationToken(commandExecutionCTS.Token, synchronizationToken);
                await PlayedCommand.ExecuteAsync(executionToken);
                executedCommandsCount--;
            }
            else
            {
                var executionToken = new CancellationToken(commandExecutionCTS.Token, synchronizationToken);
                ExecuteCommandConcurrently().Forget();
                async UniTaskVoid ExecuteCommandConcurrently ()
                {
                    await PlayedCommand.ExecuteAsync(executionToken);
                    executedCommandsCount--;
                }
            }
            if (await WaitSynchronizeAsync(cancellationToken)) return;

            for (int i = postExecutionTasks.Count - 1; i >= 0; i--)
            {
                await postExecutionTasks[i](PlayedCommand);
                if (await WaitSynchronizeAsync(cancellationToken)) return;
            }

            if (await WaitSynchronizeAsync(cancellationToken)) return;
            
            if (providerConfig.ResourcePolicy == ResourcePolicy.Dynamic)
            {
                if (PlayedCommand is Command.IPreloadable playedPreloadableCmd)
                    playedPreloadableCmd.ReleasePreloadedResources();
                if (Playlist.GetCommandByIndex(PlayedIndex + providerConfig.DynamicPolicySteps) is Command.IPreloadable nextPreloadableCmd)
                    nextPreloadableCmd.PreloadResourcesAsync().Forget();
            }

            OnCommandExecutionFinish?.Invoke(PlayedCommand);
        }

        private async UniTask PlayRoutineAsync (CancellationToken cancellationToken)
        {
            while (Engine.Initialized && Playing)
            {
                if (WaitingForInput)
                {
                    if (AutoPlayActive) 
                    { 
                        await UniTask.WhenAny(WaitForAutoPlayDelayAsync(), WaitForWaitForInputDisabledAsync()); 
                        if (await WaitSynchronizeAsync(cancellationToken)) return;
                        DisableWaitingForInput(); 
                    }
                    else
                    {
                        await WaitForWaitForInputDisabledAsync();
                        if (await WaitSynchronizeAsync(cancellationToken)) return;
                    }
                }

                await ExecutePlayedCommandAsync(cancellationToken);
                if (await WaitSynchronizeAsync(cancellationToken)) return;

                var nextActionAvailable = SelectNextCommand();
                if (!nextActionAvailable) break;

                if (SkipActive && !SkipAllowed) SetSkipEnabled(false);
            }
        }

        private async UniTask<bool> FastForwardRoutineAsync (CancellationToken cancellationToken, int targetPlaylistIndex, bool executePlayedCommand)
        {
            SetSkipEnabled(true);

            if (executePlayedCommand)
            {
                await ExecutePlayedCommandAsync(cancellationToken);
                if (await WaitSynchronizeAsync(cancellationToken)) return false;
            }

            var reachedLine = true;
            while (Engine.Initialized && Playing)
            {
                var nextCommandAvailable = SelectNextCommand();
                if (!nextCommandAvailable) { reachedLine = false; break; }

                if (PlayedIndex >= targetPlaylistIndex) { reachedLine = true; break; }

                await ExecutePlayedCommandAsync(cancellationToken);
                if (await WaitSynchronizeAsync(cancellationToken)) return false;
                SetSkipEnabled(true); // Force skip mode to be always active while fast-forwarding.

                if (cancellationToken.CancelASAP) { reachedLine = false; break; }
            }

            SetSkipEnabled(false);
            return reachedLine;
        }

        /// <summary>
        /// Attempts to select next <see cref="Command"/> in the current <see cref="Playlist"/>.
        /// </summary>
        /// <returns>Whether next command is available and was selected.</returns>
        private bool SelectNextCommand ()
        {
            PlayedIndex++;
            if (Playlist.IsIndexValid(PlayedIndex))
            {
                executedPlayedCommand = false;
                return true;
            }

            // No commands left in the played script.
            Debug.Log($"Script '{PlayedScript.Name}' has finished playing, and there wasn't a follow-up goto command. " +
                        "Consider using stop command in case you wish to gracefully stop script execution.");
            Stop();
            return false;
        }

        /// <summary>
        /// Cancels all the asynchronously-running commands.
        /// </summary>
        /// <remarks>
        /// Be aware that this could lead to an inconsistent state; only use when the current engine state is going to be discarded 
        /// (eg, when preparing to load a game or perform state rollback).
        /// </remarks>
        private void CancelCommands ()
        {
            commandExecutionCTS.Cancel();
            commandExecutionCTS.Dispose();
            commandExecutionCTS = new CancellationTokenSource();
        }
    } 
}
