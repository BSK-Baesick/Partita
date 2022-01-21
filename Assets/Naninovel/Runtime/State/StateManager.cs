// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <inheritdoc cref="IStateManager"/>
    [InitializeAtRuntime(1), Goto.DontReset] // Here settings for all the other services will be applied, so initialize at the end.
    public class StateManager : IStateManager
    {
        public event Action<GameSaveLoadArgs> OnGameLoadStarted;
        public event Action<GameSaveLoadArgs> OnGameLoadFinished;
        public event Action<GameSaveLoadArgs> OnGameSaveStarted;
        public event Action<GameSaveLoadArgs> OnGameSaveFinished;
        public event Action OnResetStarted;
        public event Action OnResetFinished;
        public event Action OnRollbackStarted;
        public event Action OnRollbackFinished;

        public virtual StateConfiguration Configuration { get; }
        public virtual GlobalStateMap GlobalState { get; private set; }
        public virtual SettingsStateMap SettingsState { get; private set; }
        public virtual ISaveSlotManager<GlobalStateMap> GlobalStateSlotManager { get; }
        public virtual ISaveSlotManager<GameStateMap> GameStateSlotManager { get; }
        public virtual ISaveSlotManager<SettingsStateMap> SettingsSlotManager { get; }
        public virtual bool QuickLoadAvailable => GameStateSlotManager.SaveSlotExists(LastQuickSaveSlotId);
        public virtual bool AnyGameSaveExists => GameStateSlotManager.AnySaveExists();
        public virtual bool RollbackInProgress => rollbackTaskQueue.Count > 0;

        protected virtual string LastQuickSaveSlotId => Configuration.IndexToQuickSaveSlotId(1);
        protected virtual StateRollbackStack RollbackStack { get; }

        private readonly Queue<GameStateMap> rollbackTaskQueue = new Queue<GameStateMap>();
        private readonly List<Action<GameStateMap>> onGameSerializeTasks = new List<Action<GameStateMap>>();
        private readonly List<Func<GameStateMap, UniTask>> onGameDeserializeTasks = new List<Func<GameStateMap, UniTask>>();
        private IInputSampler rollbackInput;
        private IScriptPlayer scriptPlayer;
        private ICameraManager cameraManager;

        public StateManager (StateConfiguration config, EngineConfiguration engineConfig)
        {
            Configuration = config;

            if (config.EnableStateRollback)
            {
                var rollbackCapacity = Mathf.Max(1, config.StateRollbackSteps);
                RollbackStack = new StateRollbackStack(rollbackCapacity);
            }

            var savesFolderPath = PathUtils.Combine(engineConfig.GeneratedDataPath, config.SaveFolderName);
            GameStateSlotManager = (ISaveSlotManager<GameStateMap>)Activator.CreateInstance(Type.GetType(config.GameStateHandler), config, savesFolderPath);
            GlobalStateSlotManager = (ISaveSlotManager<GlobalStateMap>)Activator.CreateInstance(Type.GetType(config.GlobalStateHandler), config, savesFolderPath);
            SettingsSlotManager = (ISaveSlotManager<SettingsStateMap>)Activator.CreateInstance(Type.GetType(config.SettingsStateHandler), config, savesFolderPath);
        }

        public virtual async UniTask InitializeServiceAsync ()
        {
            scriptPlayer = Engine.GetService<IScriptPlayer>();
            cameraManager = Engine.GetService<ICameraManager>();
            
            if (Configuration.EnableStateRollback)
            {
                scriptPlayer.AddPreExecutionTask(HandleCommandPreExecution);

                rollbackInput = Engine.GetService<IInputManager>().GetRollback();
                if (rollbackInput != null)
                    rollbackInput.OnStart += HandleRollbackInputStart;
            }

            SettingsState = await LoadSettingsAsync();
            GlobalState = await LoadGlobalStateAsync();
        }

        public virtual void ResetService ()
        {
            RollbackStack?.Clear();
        }

        public virtual void DestroyService ()
        {
            scriptPlayer?.RemovePreExecutionTask(HandleCommandPreExecution);

            if (rollbackInput != null)
                rollbackInput.OnStart -= HandleRollbackInputStart;
        }

        public virtual void AddOnGameSerializeTask (Action<GameStateMap> task) => onGameSerializeTasks.Insert(0, task);

        public virtual void RemoveOnGameSerializeTask (Action<GameStateMap> task) => onGameSerializeTasks.Remove(task);

        public virtual void AddOnGameDeserializeTask (Func<GameStateMap, UniTask> task) => onGameDeserializeTasks.Insert(0, task);

        public virtual void RemoveOnGameDeserializeTask (Func<GameStateMap, UniTask> task) => onGameDeserializeTasks.Remove(task);

        public virtual async UniTask<GameStateMap> SaveGameAsync (string slotId)
        {
            var quick = slotId.StartsWithFast(Configuration.QuickSaveSlotMask.GetBefore("{"));

            OnGameSaveStarted?.Invoke(new GameSaveLoadArgs(slotId, quick));
            
            var state = new GameStateMap();
            await scriptPlayer.SynchronizeAndDoAsync(DoSaveAfterSync);
            
            OnGameSaveFinished?.Invoke(new GameSaveLoadArgs(slotId, quick));

            return state;

            async UniTask DoSaveAfterSync ()
            {
                state.SaveDateTime = DateTime.Now;
                state.Thumbnail = cameraManager.CaptureThumbnail();
            
                SaveAllServicesToState<IStatefulService<GameStateMap>, GameStateMap>(state);

                for (int i = onGameSerializeTasks.Count - 1; i >= 0; i--)
                    onGameSerializeTasks[i](state);

                if (RollbackStack != null)
                {
                    // Closest spot with zero inline index is used when changing locale (UI/GameSettings/LanguageDropdown.cs).
                    var nearestStartLineSpot = RollbackStack.FirstOrDefault(s => s.PlaybackSpot.InlineIndex == 0); 
                    bool SelectSerializedSnapshots (GameStateMap s) => s.PlayerRollbackAllowed || s == nearestStartLineSpot;
                    state.RollbackStackJson = RollbackStack.ToJson(Configuration.SavedRollbackSteps, SelectSerializedSnapshots);
                }

                await GameStateSlotManager.SaveAsync(slotId, state);

                // Also save global state on every game save.
                await SaveGlobalStateAsync();
            }
        }

        public virtual async UniTask<GameStateMap> QuickSaveAsync ()
        {
            // Free first quick save slot by shifting existing ones by one.
            for (int i = Configuration.QuickSaveSlotLimit; i > 0; i--)
            {
                var curSlotId = Configuration.IndexToQuickSaveSlotId(i);
                var prevSlotId = Configuration.IndexToQuickSaveSlotId(i + 1);
                GameStateSlotManager.RenameSaveSlot(curSlotId, prevSlotId);
            }

            // Delete the last slot in case it's out of the limit.
            var outOfLimitSlotId = Configuration.IndexToQuickSaveSlotId(Configuration.QuickSaveSlotLimit + 1);
            if (GameStateSlotManager.SaveSlotExists(outOfLimitSlotId))
                GameStateSlotManager.DeleteSaveSlot(outOfLimitSlotId);

            var firstSlotId = string.Format(Configuration.QuickSaveSlotMask, 1);
            return await SaveGameAsync(firstSlotId);
        }

        public virtual async UniTask<GameStateMap> LoadGameAsync (string slotId)
        {
            if (string.IsNullOrEmpty(slotId) || !GameStateSlotManager.SaveSlotExists(slotId))
                throw new Exception($"Slot '{slotId}' not found when loading '{typeof(GameStateMap)}' data.");

            var quick = slotId.EqualsFast(LastQuickSaveSlotId);

            OnGameLoadStarted?.Invoke(new GameSaveLoadArgs(slotId, quick));

            if (Configuration.LoadStartDelay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(Configuration.LoadStartDelay));

            Engine.Reset();
            await Resources.UnloadUnusedAssets();

            var state = await GameStateSlotManager.LoadAsync(slotId);
            await LoadAllServicesFromStateAsync<IStatefulService<GameStateMap>, GameStateMap>(state);

            // All the serialized snapshots are expected to allow player rollback.
            RollbackStack?.OverrideFromJson(state.RollbackStackJson, s => s.AllowPlayerRollback());

            for (int i = onGameDeserializeTasks.Count - 1; i >= 0; i--)
                await onGameDeserializeTasks[i](state);

            OnGameLoadFinished?.Invoke(new GameSaveLoadArgs(slotId, quick));

            return state;
        }

        public virtual async UniTask<GameStateMap> QuickLoadAsync () => await LoadGameAsync(LastQuickSaveSlotId);

        public virtual async UniTask<GlobalStateMap> SaveGlobalStateAsync ()
        {
            SaveAllServicesToState<IStatefulService<GlobalStateMap>, GlobalStateMap>(GlobalState);
            await GlobalStateSlotManager.SaveAsync(Configuration.DefaultGlobalSlotId, GlobalState);
            return GlobalState;
        }

        public virtual async UniTask<SettingsStateMap> SaveSettingsAsync ()
        {
            SaveAllServicesToState<IStatefulService<SettingsStateMap>, SettingsStateMap>(SettingsState);
            await SettingsSlotManager.SaveAsync(Configuration.DefaultSettingsSlotId, SettingsState);
            return SettingsState;
        }

        public virtual async UniTask ResetStateAsync (params Func<UniTask>[] tasks)
        {
            await ResetStateAsync(default(Type[]), tasks);
        }

        public virtual async UniTask ResetStateAsync (string[] exclude, params Func<UniTask>[] tasks)
        {
            var serviceTypes = Engine.GetAllServices<IEngineService>().Select(s => s.GetType());
            var excludeTypes = serviceTypes.Where(t => exclude.Contains(t.Name) || t.GetInterfaces().Any(i => exclude.Contains(i.Name))).ToArray();
            await ResetStateAsync(excludeTypes, tasks);
        }

        public virtual async UniTask ResetStateAsync (Type[] exclude, params Func<UniTask>[] tasks)
        {
            OnResetStarted?.Invoke();

            if (Configuration.LoadStartDelay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(Configuration.LoadStartDelay));

            scriptPlayer?.Playlist?.ReleaseResources();

            Engine.Reset(exclude);

            await Resources.UnloadUnusedAssets();

            if (tasks != null)
            {
                foreach (var task in tasks)
                    if (task != null)
                        await task.Invoke();
            }

            OnResetFinished?.Invoke();
        }

        public virtual void PushRollbackSnapshot (bool allowPlayerRollback)
        {
            if (RollbackStack is null) return;
            
            var state = new GameStateMap();
            state.SaveDateTime = DateTime.Now;
            state.PlayerRollbackAllowed = allowPlayerRollback;

            SaveAllServicesToState<IStatefulService<GameStateMap>, GameStateMap>(state);

            for (int i = onGameSerializeTasks.Count - 1; i >= 0; i--)
                onGameSerializeTasks[i](state);

            RollbackStack.Push(state);
        }

        public virtual async UniTask<bool> RollbackAsync (Predicate<GameStateMap> predicate)
        {
            if (RollbackStack is null) return false;
            
            var state = RollbackStack.Pop(predicate);
            if (state is null) return false;

            await RollbackToStateAsync(state);
            return true;
        }

        public virtual async UniTask<bool> RollbackAsync ()
        {
            if (RollbackStack is null || RollbackStack.Capacity <= 1) return false;

            var state = RollbackStack.Pop();
            if (state is null) return false;

            await RollbackToStateAsync(state);
            return true;
        }

        public virtual GameStateMap PeekRollbackStack () => RollbackStack?.Peek();

        public virtual bool CanRollbackTo (Predicate<GameStateMap> predicate) => RollbackStack?.Contains(predicate) ?? false;

        public virtual void PurgeRollbackData () => RollbackStack?.ForEach(s => s.PlayerRollbackAllowed = false);

        protected virtual async UniTask RollbackToStateAsync (GameStateMap state)
        {
            rollbackTaskQueue.Enqueue(state);
            OnRollbackStarted?.Invoke();

            while (rollbackTaskQueue.Peek() != state)
                await AsyncUtils.WaitEndOfFrame;

            await LoadAllServicesFromStateAsync<IStatefulService<GameStateMap>, GameStateMap>(state);

            for (int i = onGameDeserializeTasks.Count - 1; i >= 0; i--)
                await onGameDeserializeTasks[i](state);

            rollbackTaskQueue.Dequeue();
            OnRollbackFinished?.Invoke();
        }

        private async UniTask<GlobalStateMap> LoadGlobalStateAsync ()
        {
            var stateData = await GlobalStateSlotManager.LoadOrDefaultAsync(Configuration.DefaultGlobalSlotId);
            await LoadAllServicesFromStateAsync<IStatefulService<GlobalStateMap>, GlobalStateMap>(stateData);
            return stateData;
        }

        private async UniTask<SettingsStateMap> LoadSettingsAsync ()
        {
            var settingsData = await SettingsSlotManager.LoadOrDefaultAsync(Configuration.DefaultSettingsSlotId);
            await LoadAllServicesFromStateAsync<IStatefulService<SettingsStateMap>, SettingsStateMap>(settingsData);
            return settingsData;
        }

        private void SaveAllServicesToState<TService, TState> (TState state) 
            where TService : class, IStatefulService<TState>
            where TState : StateMap, new()
        {
            foreach (var service in Engine.GetAllServices<TService>())
                service.SaveServiceState(state);
        }

        private async UniTask LoadAllServicesFromStateAsync<TService, TState> (TState state)
            where TService : class, IStatefulService<TState>
            where TState : StateMap, new()
        {
            foreach (var service in Engine.GetAllServices<TService>())
                await service.LoadServiceStateAsync(state);
        }

        private async void HandleRollbackInputStart ()
        {
            if (!Configuration.EnableStateRollback || !CanRollbackTo(s => s.PlayerRollbackAllowed)) return;

            await RollbackAsync(s => s.PlayerRollbackAllowed);
        }

        private UniTask HandleCommandPreExecution (Commands.Command _)
        {
            PushRollbackSnapshot(false);
            return UniTask.CompletedTask;
        }
    } 
}
