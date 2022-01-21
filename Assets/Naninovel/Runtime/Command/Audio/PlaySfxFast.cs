// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel.Commands
{
    /// <summary>
    /// Plays an [SFX (sound effect)](/guide/audio.md#sound-effects) track with the provided name.
    /// Unlike [@sfx] command, the clip is played with minimum delay and is not serialized with the game state (won't be played after loading a game, even if it was played when saved).
    /// The command can be used to play various transient audio clips, such as UI-related sounds (eg, on button click with [`Play Script` component](/guide/user-interface.md#play-script-on-unity-event)).
    /// </summary>
    /// <example>
    /// ; Plays an SFX with the name `Click` once
    /// @sfxFast Click
    /// 
    /// ; Same as above, but allow concurrent playbacks of the same clip
    /// @sfxFast Click restart:false
    /// </example>
    [CommandAlias("sfxFast")]
    public class PlaySfxFast : AudioCommand, Command.IPreloadable
    {
        /// <summary>
        /// Path to the sound effect asset to play.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), IDEResource(AudioConfiguration.DefaultAudioPathPrefix)]
        public StringParameter SfxPath;
        /// <summary>
        /// Volume of the sound effect.
        /// </summary>
        public DecimalParameter Volume = 1f;
        /// <summary>
        /// Whether to start playing the audio from start in case it's already playing.
        /// </summary>
        public BooleanParameter Restart = true;
        /// <summary>
        /// Whether to allow playing multiple instances of the same clip; has no effect when `restart` is enabled.
        /// </summary>
        public BooleanParameter Additive = true;
        /// <summary>
        /// Audio mixer [group path](https://docs.unity3d.com/ScriptReference/Audio.AudioMixer.FindMatchingGroups) that should be used when playing the audio.
        /// </summary>
        [ParameterAlias("group")]
        public StringParameter GroupPath;

        public async UniTask PreloadResourcesAsync ()
        {
            if (!Assigned(SfxPath) || SfxPath.DynamicValue) return;
            await AudioManager.AudioLoader.LoadAndHoldAsync(SfxPath, this);
        }

        public void ReleasePreloadedResources ()
        {
            if (!Assigned(SfxPath) || SfxPath.DynamicValue) return;
            AudioManager?.AudioLoader?.Release(SfxPath, this);
        }

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            // Make sure the resource is loaded, it won't play otherwise.
            if (!AudioManager.AudioLoader.IsLoaded(SfxPath))
                await AudioManager.AudioLoader.LoadAsync(SfxPath);
            AudioManager.PlaySfxFast(SfxPath, Volume, GroupPath, Restart, Additive);   
        }
        
    } 
}
