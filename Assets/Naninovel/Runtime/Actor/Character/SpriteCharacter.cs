// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="ICharacterActor"/> implementation using <see cref="SpriteActor{TMeta}"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(Texture2D), true)]
    public class SpriteCharacter : SpriteActor<CharacterMetadata>, ICharacterActor
    {
        public CharacterLookDirection LookDirection { get => GetLookDirection(); set => SetLookDirection(value); }

        private readonly CharacterLookDirection bakedLookDirection;

        public SpriteCharacter (string id, CharacterMetadata metadata)
            : base(id, metadata)
        {
            bakedLookDirection = metadata.BakedLookDirection;
        }

        public UniTask ChangeLookDirectionAsync (CharacterLookDirection lookDirection, float duration, 
            EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            SetLookDirection(lookDirection);
            return UniTask.CompletedTask;
        }

        protected virtual void SetLookDirection (CharacterLookDirection lookDirection)
        {
            if (bakedLookDirection == CharacterLookDirection.Center) return;
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
            switch (bakedLookDirection)
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
