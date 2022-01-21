﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Audio;

namespace Naninovel
{
    /// <inheritdoc cref="IAudioManager"/>
    [InitializeAtRuntime]
    public class AudioManager : IStatefulService<SettingsStateMap>, IStatefulService<GameStateMap>, IAudioManager
    {
        [Serializable]
        public class Settings
        {
            public float MasterVolume;
            public float BgmVolume;
            public float SfxVolume;
            public float VoiceVolume;
            public string VoiceLocale;
            public List<NamedFloat> AuthorVolume;
        }

        [Serializable]
        public class GameState { public List<ClipState> BgmClips; public List<ClipState> SfxClips; }

        [Serializable]
        public struct ClipState { public string Path; public float Volume; public bool IsLooped; }

        public virtual AudioConfiguration Configuration { get; }
        public virtual AudioListener AudioListener { get; private set; }
        public virtual AudioMixer AudioMixer { get; }
        public virtual float MasterVolume { get => GetMixerVolume(Configuration.MasterVolumeHandleName); set => SetMixerVolume(Configuration.MasterVolumeHandleName, value); }
        public virtual float BgmVolume { get => GetMixerVolume(Configuration.BgmVolumeHandleName); set { if (BgmGroupAvailable) SetMixerVolume(Configuration.BgmVolumeHandleName, value); } }
        public virtual float SfxVolume { get => GetMixerVolume(Configuration.SfxVolumeHandleName); set { if (SfxGroupAvailable) SetMixerVolume(Configuration.SfxVolumeHandleName, value); } }
        public virtual float VoiceVolume { get => GetMixerVolume(Configuration.VoiceVolumeHandleName); set { if (VoiceGroupAvailable) SetMixerVolume(Configuration.VoiceVolumeHandleName, value); } }
        public virtual string VoiceLocale { get => voiceLoader.OverrideLocale; set => voiceLoader.OverrideLocale = value; }
        public virtual IResourceLoader<AudioClip> AudioLoader => audioLoader;
        public virtual IResourceLoader<AudioClip> VoiceLoader => voiceLoader;

        protected virtual bool BgmGroupAvailable => bgmGroup;
        protected virtual bool SfxGroupAvailable => sfxGroup;
        protected virtual bool VoiceGroupAvailable => voiceGroup;

        private readonly IResourceProviderManager providerManager;
        private readonly ILocalizationManager localizationManager;
        private readonly Dictionary<string, ClipState> bgmMap, sfxMap;
        private readonly Dictionary<string, float> authorVolume;
        private AudioMixerGroup bgmGroup, sfxGroup, voiceGroup;
        private LocalizableResourceLoader<AudioClip> audioLoader, voiceLoader;
        private AudioController audioController;
        private ClipState? voiceClip;

        public AudioManager (AudioConfiguration config, IResourceProviderManager providerManager, ILocalizationManager localizationManager)
        {
            Configuration = config;
            this.providerManager = providerManager;
            this.localizationManager = localizationManager;

            AudioMixer = ObjectUtils.IsValid(config.CustomAudioMixer) ? config.CustomAudioMixer : Resources.Load<AudioMixer>(AudioConfiguration.DefaultMixerResourcesPath);
            
            bgmMap = new Dictionary<string, ClipState>();
            sfxMap = new Dictionary<string, ClipState>();
            authorVolume = new Dictionary<string, float>();
        }

        public virtual UniTask InitializeServiceAsync ()
        {
            if (ObjectUtils.IsValid(AudioMixer))
            {
                bgmGroup = AudioMixer.FindMatchingGroups(Configuration.BgmGroupPath)?.FirstOrDefault();
                sfxGroup = AudioMixer.FindMatchingGroups(Configuration.SfxGroupPath)?.FirstOrDefault();
                voiceGroup = AudioMixer.FindMatchingGroups(Configuration.VoiceGroupPath)?.FirstOrDefault();
            }
            
            audioLoader = Configuration.AudioLoader.CreateLocalizableFor<AudioClip>(providerManager, localizationManager);
            voiceLoader = Configuration.VoiceLoader.CreateLocalizableFor<AudioClip>(providerManager, localizationManager);
            audioController = Engine.CreateObject<AudioController>();

            AudioListener = audioController.Listener;

            return UniTask.CompletedTask;
        }

        public virtual void ResetService ()
        {
            audioController.StopAllClips();
            bgmMap.Clear();
            sfxMap.Clear();
            voiceClip = null;

            audioLoader?.ReleaseAll(this);
            voiceLoader?.ReleaseAll(this);
        }

        public virtual void DestroyService ()
        {
            if (audioController)
            {
                audioController.StopAllClips();
                UnityEngine.Object.Destroy(audioController.gameObject);
            }

            audioLoader?.ReleaseAll(this);
            voiceLoader?.ReleaseAll(this);
        }

        public virtual void SaveServiceState (SettingsStateMap stateMap)
        {
            var settings = new Settings {
                MasterVolume = MasterVolume,
                BgmVolume = BgmVolume,
                SfxVolume = SfxVolume,
                VoiceVolume = VoiceVolume,
                VoiceLocale = VoiceLocale,
                AuthorVolume = authorVolume.Select(kv => new NamedFloat(kv.Key, kv.Value)).ToList()
            };
            stateMap.SetState(settings);
        }

        public virtual UniTask LoadServiceStateAsync (SettingsStateMap stateMap)
        {
            var settings = stateMap.GetState<Settings>();

            authorVolume.Clear();

            if (settings is null) // Apply default settings.
            {
                MasterVolume = Configuration.DefaultMasterVolume;
                BgmVolume = Configuration.DefaultBgmVolume;
                SfxVolume = Configuration.DefaultSfxVolume;
                VoiceVolume = Configuration.DefaultVoiceVolume;
                VoiceLocale = Configuration.VoiceLocales?.FirstOrDefault();
                return UniTask.CompletedTask;
            }

            MasterVolume = settings.MasterVolume;
            BgmVolume = settings.BgmVolume;
            SfxVolume = settings.SfxVolume;
            VoiceVolume = settings.VoiceVolume;
            VoiceLocale = Configuration.VoiceLocales?.Count > 0 ? settings.VoiceLocale ?? Configuration.VoiceLocales.First() : null;

            foreach (var item in settings.AuthorVolume)
                authorVolume[item.Name] = item.Value;

            return UniTask.CompletedTask;
        }

        public virtual void SaveServiceState (GameStateMap stateMap)
        {
            var state = new GameState { // Save only looped audio to prevent playing multiple clips at once when the game is (auto) saved in skip mode.
                BgmClips = bgmMap.Values.Where(s => IsBgmPlaying(s.Path) && s.IsLooped).ToList(),
                SfxClips = sfxMap.Values.Where(s => IsSfxPlaying(s.Path) && s.IsLooped).ToList()
            };
            stateMap.SetState(state);
        }

        public virtual async UniTask LoadServiceStateAsync (GameStateMap stateMap)
        {
            var state = stateMap.GetState<GameState>() ?? new GameState();
            var tasks = new List<UniTask>();

            if (state.BgmClips != null && state.BgmClips.Count > 0)
            {
                foreach (var bgmPath in bgmMap.Keys.ToList())
                    if (!state.BgmClips.Exists(c => c.Path.EqualsFast(bgmPath)))
                        tasks.Add(StopBgmAsync(bgmPath));
                foreach (var clipState in state.BgmClips)
                    if (IsBgmPlaying(clipState.Path))
                        tasks.Add(ModifyBgmAsync(clipState.Path, clipState.Volume, clipState.IsLooped, 0));
                    else tasks.Add(PlayBgmAsync(clipState.Path, clipState.Volume, 0, clipState.IsLooped));
            }
            else tasks.Add(StopAllBgmAsync());

            if (state.SfxClips != null && state.SfxClips.Count > 0)
            {
                foreach (var sfxPath in sfxMap.Keys.ToList())
                    if (!state.SfxClips.Exists(c => c.Path.EqualsFast(sfxPath)))
                        tasks.Add(StopSfxAsync(sfxPath));
                foreach (var clipState in state.SfxClips)
                    if (IsSfxPlaying(clipState.Path))
                        tasks.Add(ModifySfxAsync(clipState.Path, clipState.Volume, clipState.IsLooped, 0));
                    else tasks.Add(PlaySfxAsync(clipState.Path, clipState.Volume, 0, clipState.IsLooped));
            }
            else tasks.Add(StopAllSfxAsync());

            await UniTask.WhenAll(tasks);
        }

        public virtual IReadOnlyCollection<string> GetPlayedBgmPaths () => bgmMap.Keys.Where(IsBgmPlaying).ToArray();

        public virtual IReadOnlyCollection<string> GetPlayedSfxPaths () => sfxMap.Keys.Where(IsSfxPlaying).ToArray();

        public virtual string GetPlayedVoicePath () => IsVoicePlaying(voiceClip?.Path) ? voiceClip?.Path : null;

        public virtual async UniTask<bool> AudioExistsAsync (string path) => await audioLoader.ExistsAsync(path);

        public virtual async UniTask<bool> VoiceExistsAsync (string path) => await voiceLoader.ExistsAsync(path);

        public virtual async UniTask ModifyBgmAsync (string path, float volume, bool loop, float time, CancellationToken cancellationToken = default)
        {
            if (!bgmMap.ContainsKey(path)) return;

            var state = bgmMap[path];
            state.Volume = volume;
            state.IsLooped = loop;
            bgmMap[path] = state;
            await ModifyAudioAsync(path, volume, loop, time, cancellationToken);
        }

        public virtual async UniTask ModifySfxAsync (string path, float volume, bool loop, float time, CancellationToken cancellationToken = default)
        {
            if (!sfxMap.ContainsKey(path)) return;

            var state = sfxMap[path];
            state.Volume = volume;
            state.IsLooped = loop;
            sfxMap[path] = state;
            await ModifyAudioAsync(path, volume, loop, time, cancellationToken);
        }

        public virtual void PlaySfxFast (string path, float volume = 1f, string group = default, bool restart = true, bool additive = true)
        {
            if (!audioLoader.IsLoaded(path))
                throw new Exception($"Failed to fast-play `{path}` SFX: the associated audio clip resource is not loaded.");
            var clip = audioLoader.GetLoadedOrNull(path);
            if (audioController.ClipPlaying(clip) && !restart && !additive) return;
            audioController.PlayClip(clip, null, volume, false, FindAudioGroupOrDefault(group, sfxGroup), null, additive);
        }

        public virtual async UniTask PlayBgmAsync (string path, float volume = 1f, float fadeTime = 0f, bool loop = true, string introPath = null, string group = default, CancellationToken cancellationToken = default)
        {
            var clipResource = await audioLoader.LoadAndHoldAsync(path, this);
            if (cancellationToken.CancelASAP) return;
            if (!clipResource.Valid)
            {
                Debug.LogWarning($"Failed to play BGM `{path}`: resource not found.");
                return;
            }

            bgmMap[path] = new ClipState { Path = path, Volume = volume, IsLooped = loop };

            var introClip = default(AudioClip);
            if (!string.IsNullOrEmpty(introPath))
            {
                var introClipResource = await audioLoader.LoadAndHoldAsync(introPath, this);
                if (!introClipResource.Valid)
                    Debug.LogWarning($"Failed to load intro BGM `{path}`: resource not found.");
                else introClip = introClipResource.Object;
            }

            if (fadeTime <= 0) audioController.PlayClip(clipResource, null, volume, loop, FindAudioGroupOrDefault(group, bgmGroup), introClip);
            else await audioController.PlayClipAsync(clipResource, fadeTime, null, volume, loop, FindAudioGroupOrDefault(group, bgmGroup), introClip, cancellationToken: cancellationToken);
        }

        public virtual async UniTask StopBgmAsync (string path, float fadeTime = 0f, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (bgmMap.ContainsKey(path))
                bgmMap.Remove(path);

            if (!audioLoader.IsLoaded(path)) return;
            var clipResource = audioLoader.GetLoadedOrNull(path);
            if (fadeTime <= 0) audioController.StopClip(clipResource);
            else await audioController.StopClipAsync(clipResource, fadeTime, cancellationToken);

            if (!IsBgmPlaying(path))
                audioLoader?.Release(path, this);
        }

        public virtual async UniTask StopAllBgmAsync (float fadeTime = 0f, CancellationToken cancellationToken = default)
        {
            await UniTask.WhenAll(bgmMap.Keys.ToList().Select(p => StopBgmAsync(p, fadeTime, cancellationToken)));
        }

        public virtual async UniTask PlaySfxAsync (string path, float volume = 1f, float fadeTime = 0f, bool loop = false, string group = default, CancellationToken cancellationToken = default)
        {
            var clipResource = await audioLoader.LoadAndHoldAsync(path, this);
            if (cancellationToken.CancelASAP) return;
            if (!clipResource.Valid)
            {
                Debug.LogWarning($"Failed to play SFX `{path}`: resource not found.");
                return;
            }

            sfxMap[path] = new ClipState { Path = path, Volume = volume, IsLooped = loop };

            if (fadeTime <= 0) audioController.PlayClip(clipResource, null, volume, loop, FindAudioGroupOrDefault(group, sfxGroup));
            else await audioController.PlayClipAsync(clipResource, fadeTime, null, volume, loop, FindAudioGroupOrDefault(group, sfxGroup), cancellationToken: cancellationToken);
        }

        public virtual async UniTask StopSfxAsync (string path, float fadeTime = 0f, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (sfxMap.ContainsKey(path))
                sfxMap.Remove(path);

            if (!audioLoader.IsLoaded(path)) return;
            var clipResource = audioLoader.GetLoadedOrNull(path);
            if (fadeTime <= 0) audioController.StopClip(clipResource);
            else await audioController.StopClipAsync(clipResource, fadeTime, cancellationToken);

            if (!IsSfxPlaying(path))
                audioLoader?.Release(path, this);
        }

        public virtual async UniTask StopAllSfxAsync (float fadeTime = 0f, CancellationToken cancellationToken = default)
        {
            await UniTask.WhenAll(sfxMap.Keys.ToList().Select(p => StopSfxAsync(p, fadeTime, cancellationToken)));
        }

        public virtual async UniTask PlayVoiceAsync (string path, float volume = 1f, string group = default, CancellationToken cancellationToken = default)
        {
            var clipResource = await voiceLoader.LoadAndHoldAsync(path, this);
            if (!clipResource.Valid || cancellationToken.CancelASAP) return;

            if (Configuration.VoiceOverlapPolicy == VoiceOverlapPolicy.PreventOverlap)
                StopVoice();

            voiceClip = new ClipState { Path = path, IsLooped = false, Volume = volume };

            audioController.PlayClip(clipResource, volume: volume, mixerGroup: FindAudioGroupOrDefault(group, voiceGroup));
        }

        public virtual async UniTask PlayVoiceSequenceAsync (List<string> pathList, float volume = 1f, string group = default, CancellationToken cancellationToken = default)
        {
            foreach (var path in pathList)
            {
                await PlayVoiceAsync(path, volume, group);
                if (cancellationToken.CancelASAP) return;
                await UniTask.WaitWhile(() => IsVoicePlaying(path));
                if (cancellationToken.CancelASAP) return;
            }
        }

        public virtual void StopVoice ()
        {
            if (!voiceClip.HasValue) return;

            var clipResource = voiceLoader.GetLoadedOrNull(voiceClip.Value.Path);
            audioController.StopClip(clipResource);
            voiceLoader.Release(voiceClip.Value.Path, this);
            voiceClip = null;
        }

        public virtual IAudioTrack GetAudioTrack (string path)
        {
            var clipResource = audioLoader.GetLoadedOrNull(path);
            if (clipResource is null || !clipResource.Valid) return null;
            return audioController.GetTracks(clipResource.Object)?.FirstOrDefault();
        }

        public virtual IAudioTrack GetVoiceTrack (string path)
        {
            var clipResource = voiceLoader.GetLoadedOrNull(path);
            if (clipResource is null || !clipResource.Valid) return null;
            return audioController.GetTracks(clipResource.Object)?.FirstOrDefault();
        }

        public virtual float GetAuthorVolume (string authorId)
        {
            if (string.IsNullOrEmpty(authorId)) return -1;
            else return authorVolume.TryGetValue(authorId, out var result) ? result : -1;
        }

        public virtual void SetAuthorVolume (string authorId, float volume)
        {
            if (string.IsNullOrEmpty(authorId)) return;
            authorVolume[authorId] = volume;
        }

        private bool IsAudioPlaying (string path)
        {
            if (!audioLoader.IsLoaded(path)) return false;
            var clipResource = audioLoader.GetLoadedOrNull(path);
            if (!clipResource.Valid) return false;
            return audioController.GetTracks(clipResource)?.FirstOrDefault()?.Playing ?? false;
        }

        private async UniTask ModifyAudioAsync (string path, float volume, bool loop, float time, CancellationToken cancellationToken = default)
        {
            if (!audioLoader.IsLoaded(path)) return;
            var clipResource = audioLoader.GetLoadedOrNull(path);
            if (!clipResource.Valid) return;
            var track = audioController.GetTracks(clipResource)?.FirstOrDefault();
            if (track is null) return;
            track.Loop = loop;
            if (time <= 0) track.Volume = volume;
            else await track.FadeAsync(volume, time, cancellationToken);
        }

        private float GetMixerVolume (string handleName)
        {
            float value;

            if (ObjectUtils.IsValid(AudioMixer))
            {
                AudioMixer.GetFloat(handleName, out value);
                value = MathUtils.DecibelToLinear(value);
            }
            else value = audioController.Volume;

            return value;
        }

        private void SetMixerVolume (string handleName, float value)
        {
            if (ObjectUtils.IsValid(AudioMixer))
                AudioMixer.SetFloat(handleName, MathUtils.LinearToDecibel(value));
            else audioController.Volume = value;
        }

        private AudioMixerGroup FindAudioGroupOrDefault (string path, AudioMixerGroup defaultGroup)
        {
            if (string.IsNullOrEmpty(path)) 
                return defaultGroup;
            var group = AudioMixer.FindMatchingGroups(path)?.FirstOrDefault();
            return ObjectUtils.IsValid(group) ? group : defaultGroup;
        }
        
        private bool IsBgmPlaying (string path)
        {
            if (!bgmMap.ContainsKey(path)) return false;
            return IsAudioPlaying(path);
        }

        private bool IsSfxPlaying (string path)
        {
            if (!sfxMap.ContainsKey(path)) return false;
            return IsAudioPlaying(path);
        }

        private bool IsVoicePlaying (string path)
        {
            if (!voiceClip.HasValue || voiceClip.Value.Path != path) return false;
            if (!voiceLoader.IsLoaded(path)) return false;
            var clipResource = voiceLoader.GetLoadedOrNull(path);
            if (!clipResource.Valid) return false;
            return audioController.GetTracks(clipResource)?.FirstOrDefault()?.Playing ?? false;
        }
    }
}
