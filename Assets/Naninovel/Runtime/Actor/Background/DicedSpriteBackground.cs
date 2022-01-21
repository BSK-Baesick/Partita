// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

#if SPRITE_DICING_AVAILABLE

using SpriteDicing;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IBackgroundActor"/> implementation using "SpriteDicing" extension to represent the actor.
    /// </summary>
    [ActorResources(typeof(DicedSpriteAtlas), false)]
    public class DicedSpriteBackground : DicedSpriteActor<BackgroundMetadata>, IBackgroundActor
    {
        public DicedSpriteBackground (string id, BackgroundMetadata metadata) 
            : base(id, metadata) { }

    } 
}

#endif
