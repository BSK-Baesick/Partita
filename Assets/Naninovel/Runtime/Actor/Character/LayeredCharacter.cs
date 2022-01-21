// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="ICharacterActor"/> implementation using <see cref="LayeredActorBehaviour"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(LayeredCharacterBehaviour), false)]
    public class LayeredCharacter : LayeredActor<LayeredCharacterBehaviour, CharacterMetadata>, ICharacterActor, Commands.LipSync.IReceiver
    {
        public CharacterLookDirection LookDirection { get => GetLookDirection(); set => SetLookDirection(value); }

        private readonly ITextPrinterManager textPrinterManager;
        private readonly IAudioManager audioManager;
        private bool lipSyncAllowed = true;

        public LayeredCharacter (string id, CharacterMetadata metadata)
            : base(id, metadata)
        {
            textPrinterManager = Engine.GetService<ITextPrinterManager>();
            textPrinterManager.OnPrintTextStarted += HandlePrintTextStarted;
            audioManager = Engine.GetService<IAudioManager>();
        }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            
            Behaviour.NotifyIsSpeakingChanged(false);
        }

        public UniTask ChangeLookDirectionAsync (CharacterLookDirection lookDirection, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            SetLookDirection(lookDirection);
            return UniTask.CompletedTask;
        }
        
        public override void Dispose ()
        {
            base.Dispose();

            if (textPrinterManager != null)
            {
                textPrinterManager.OnPrintTextStarted -= HandlePrintTextStarted;
                textPrinterManager.OnPrintTextFinished -= HandlePrintTextFinished;
            }
        }

        public void AllowLipSync (bool active)
        {
            lipSyncAllowed = active;
        }

        protected virtual void SetLookDirection (CharacterLookDirection lookDirection)
        {
            if (ActorMetadata.BakedLookDirection == CharacterLookDirection.Center) return;
            if (lookDirection == CharacterLookDirection.Center)
            {
                TransitionalRenderer.FlipX = false;
                return;
            }
            if (lookDirection != LookDirection)
                TransitionalRenderer.FlipX = !TransitionalRenderer.FlipX;
        }

        protected virtual CharacterLookDirection GetLookDirection ()
        {
            switch (ActorMetadata.BakedLookDirection)
            {
                case CharacterLookDirection.Center:
                    return CharacterLookDirection.Center;
                case CharacterLookDirection.Left:
                    return TransitionalRenderer.FlipX ? CharacterLookDirection.Right : CharacterLookDirection.Left;
                case CharacterLookDirection.Right:
                    return TransitionalRenderer.FlipX ? CharacterLookDirection.Left : CharacterLookDirection.Right;
                default:
                    return default;
            }
        }
        
        private void HandlePrintTextStarted (PrintTextArgs args)
        {
            if (!lipSyncAllowed || args.AuthorId != Id) return;

            Behaviour.NotifyIsSpeakingChanged(true);

            var playedVoicePath = audioManager.GetPlayedVoicePath();
            if (!string.IsNullOrEmpty(playedVoicePath))
            {
                var track = audioManager.GetVoiceTrack(playedVoicePath);
                track.OnStop -= HandleVoiceClipStopped;
                track.OnStop += HandleVoiceClipStopped;
            }
            else textPrinterManager.OnPrintTextFinished += HandlePrintTextFinished;
        }

        private void HandlePrintTextFinished (PrintTextArgs args)
        {
            if (args.AuthorId != Id) return;
            
            Behaviour.NotifyIsSpeakingChanged(false);
            textPrinterManager.OnPrintTextFinished -= HandlePrintTextFinished;
        }

        private void HandleVoiceClipStopped ()
        {
            Behaviour.NotifyIsSpeakingChanged(false);
        }
    }
}
