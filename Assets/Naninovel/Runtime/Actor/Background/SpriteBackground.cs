// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IBackgroundActor"/> implementation using <see cref="SpriteActor{TMeta}"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(Texture2D), true)]
    public class SpriteBackground : SpriteActor<BackgroundMetadata>, IBackgroundActor
    {
        public SpriteBackground (string id, BackgroundMetadata metadata) 
            : base(id, metadata) { }

    } 
}
