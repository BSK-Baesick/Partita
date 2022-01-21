// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Plays a voice clip at the provided path.
    /// </summary>
    [CommandAlias("voice")]
    public class PlayVoice : AudioCommand, Command.IPreloadable
    {
        /// <summary>
        /// Path to the voice clip to play.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), RequiredParameter, IDEResource(AudioConfiguration.DefaultVoicePathPrefix)]
        public StringParameter VoicePath;
        /// <summary>
        /// Volume of the playback.
        /// </summary>
        public DecimalParameter Volume = 1f;
        /// <summary>
        /// Audio mixer [group path](https://docs.unity3d.com/ScriptReference/Audio.AudioMixer.FindMatchingGroups) that should be used when playing the audio.
        /// </summary>
        [ParameterAlias("group")]
        public StringParameter GroupPath;
        /// <summary>
        /// ID of the character actor this voice belongs to.
        /// When provided and [per-author volume](/guide/voicing.md#author-volume) is used, volume will be adjusted accordingly.
        /// </summary>
        public StringParameter AuthorId;

        public async UniTask PreloadResourcesAsync ()
        {
            if (!Assigned(VoicePath) || VoicePath.DynamicValue) return;
            await AudioManager.VoiceLoader.LoadAndHoldAsync(VoicePath, this);
        }

        public void ReleasePreloadedResources ()
        {
            if (!Assigned(VoicePath) || VoicePath.DynamicValue) return;
            AudioManager?.VoiceLoader?.Release(VoicePath, this);
        }

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var volume = Volume.Value;
            if (Assigned(AuthorId))
            {
                var authorVolume = AudioManager.GetAuthorVolume(AuthorId);
                if (Mathf.Approximately(authorVolume, -1)) LogWarningWithPosition($"Failed to modify @voice volume: volume for `{AuthorId}` author is not assigned.");
                else volume *= authorVolume;
            }
            await AudioManager.PlayVoiceAsync(VoicePath, volume, GroupPath);
        }
    }
}
