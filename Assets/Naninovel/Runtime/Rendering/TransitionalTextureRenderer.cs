// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="TransitionalRenderer"/> implementation, that outputs the result to a render texture.
    /// </summary>
    public class TransitionalTextureRenderer : TransitionalRenderer
    {
        /// <summary>
        /// Render texture to output the render result.
        /// </summary>
        public virtual RenderTexture RenderTexture { get; set; }
        /// <summary>
        /// Whether to resize source texture when it has different aspect with the render texture.
        /// </summary>
        public virtual bool CorrectAspect { get; set; }
        
        // Flip shouldn't affect content rendered to a texture (same as IActor.Position, etc).
        public override bool FlipX { get; set; }
        public override bool FlipY { get; set; }

        protected override string DefaultShaderName => "Naninovel/TransitionalTexture";
        
        private float opacityLastFrame = 1f;

        protected virtual void Update ()
        {
            if (!MainTexture || !RenderTexture) return;
                
            // Don't render when the content is transparent.
            if (opacityLastFrame <= 0 && Opacity <= 0) return;

            var sourceAspect = MainTexture.width / (float)MainTexture.height;
            var targetAspect = RenderTexture.width / (float)RenderTexture.height;
            var adjustedHeight = RenderTexture.width * (MainTexture.height / (float)MainTexture.width);
            var adjustedWidth = RenderTexture.height * (MainTexture.width / (float)MainTexture.height);
            var offsetX = CorrectAspect && targetAspect > sourceAspect ? (RenderTexture.width - adjustedWidth) / 2f : 0;
            var offsetY = CorrectAspect && targetAspect < sourceAspect ? (RenderTexture.height - adjustedHeight) / 2f : 0;

            Graphics.SetRenderTarget(RenderTexture);
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, RenderTexture.width, 0, RenderTexture.height);
            Material.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(offsetX, offsetY, 0);
            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(RenderTexture.width - offsetX, offsetY, 0);
            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(RenderTexture.width - offsetX, RenderTexture.height - offsetY, 0);
            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(offsetX, RenderTexture.height - offsetY, 0);
            GL.End();
            GL.PopMatrix();

            opacityLastFrame = Opacity;
        }
    } 
}
