// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

#if SPRITE_DICING_AVAILABLE

using SpriteDicing;
using UniRx.Async;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="ICharacterActor"/> implementation using "SpriteDicing" extension to represent the actor.
    /// </summary>
    [ActorResources(typeof(DicedSpriteAtlas), false)]
    public class DicedSpriteCharacter : DicedSpriteActor<CharacterMetadata>, ICharacterActor
    {
        public CharacterLookDirection LookDirection { get => GetLookDirection(); set => SetLookDirection(value); }

        public DicedSpriteCharacter (string id, CharacterMetadata metadata)
            : base(id, metadata) { }
        
        public UniTask ChangeLookDirectionAsync (CharacterLookDirection lookDirection, float duration, 
            EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            SetLookDirection(lookDirection);
            return UniTask.CompletedTask;
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
    }
}

#endif
