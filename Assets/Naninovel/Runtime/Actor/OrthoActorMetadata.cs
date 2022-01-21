// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Represents serializable data required to construct and initialize a <see cref="IActor"/> 
    /// managed in the orthographic scene space.
    /// </summary>
    [System.Serializable]
    public abstract class OrthoActorMetadata : ActorMetadata
    {
        [Tooltip("Pivot point of the actor.")]
        public Vector2 Pivot = Vector2.zero;
        [Tooltip("PPU value of the actor.")]
        public int PixelsPerUnit = 100;
        [Tooltip("Whether to perform an additional render pass writing to z-buffer. Required for some effects (eg, depth of field).")]
        public bool EnableDepthPass = true;
        [Range(0.001f, 0.999f), Tooltip("When depth pass is enabled, controls the transparency level (alpha) threshold to cut off rendered pixels of the depth mask.")]
        public float DepthAlphaCutoff = 0.5f;
        [Tooltip("A custom shader to render the actor. Be aware, that the shader is expected to have several specific properties and passes; check default built-in shader (Naninovel/Resources/Naninovel/Shaders/TransitionalSprite) for a reference.")]
        public Shader CustomShader = default;
        [Tooltip("When assigned and supported by the implementation, the actor will be rendered to the texture instead of a game object on scene.")]
        public RenderTexture RenderTexture = default;
        [Tooltip("When rendering to a texture, enable to automatically correct aspect ratio of the source texture to fit it inside the render texture.")]
        public bool CorrectRenderAspect = true;
    }
}
