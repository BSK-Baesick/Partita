// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="TransitionalRenderer"/> implementation, that outputs the result to a quad mesh (sprite).
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), DisallowMultipleComponent]
    public class TransitionalSpriteRenderer : TransitionalRenderer
    {
        public override Texture MainTexture { get => base.MainTexture; set { base.MainTexture = value; RebuildMeshQuad(); } }
        public override Texture TransitionTexture { get => base.TransitionTexture; set { base.TransitionTexture = value; RebuildMeshQuad(); } }
        public virtual Vector2 Pivot { get => pivot; set { if (value != Pivot) { pivot = value; RebuildMeshQuad(); } } }
        public virtual int PixelsPerUnit { get => pixelsPerUnit; set { if (value != PixelsPerUnit) { pixelsPerUnit = value; RebuildMeshQuad(); } } }

        protected override string DefaultShaderName => "Naninovel/TransitionalSprite";

        private readonly List<Vector3> vertices = new Vector3[4].ToList();
        private readonly List<Vector2> mainUVs = new Vector2[4].ToList();
        private readonly List<Vector2> transitionUVs = new Vector2[4].ToList();
        private readonly List<int> triangles = new List<int> { 0, 1, 2, 3, 2, 1 };
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Vector2 pivot;
        private int pixelsPerUnit;

        /// <inheritdoc cref="TransitionalRenderer.Initialize"/>
        /// <param name="pivot">Pivot (anchors) of the sprite.</param>
        /// <param name="pixelsPerUnit">How many texture pixels correspond to one unit of the sprite geometry.</param>
        public virtual void Initialize (Vector2 pivot, int pixelsPerUnit, Shader customShader = default)
        {
            Initialize(customShader);

            this.pivot = pivot;
            this.pixelsPerUnit = pixelsPerUnit;
            
            TryGetComponent<MeshFilter>(out meshFilter);
            if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.hideFlags = HideFlags.HideInInspector;
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            meshFilter.mesh.name = "Generated Quad Mesh (Instance)";
            
            TryGetComponent<MeshRenderer>(out meshRenderer);
            if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.hideFlags = HideFlags.HideInInspector;
            meshRenderer.materials = new Material[] {
                Material,
                DepthMaterial
            };
        }

        protected new void Initialize (Shader customShader)
        {
            base.Initialize(customShader);
        }

        protected virtual void OnEnable ()
        {
            if (meshRenderer)
                meshRenderer.enabled = true;
        }

        protected virtual void OnDisable ()
        {
            if (meshRenderer)
                meshRenderer.enabled = false;
        }

        private void RebuildMeshQuad ()
        {
            if (!meshFilter || !MainTexture) return;

            meshFilter.mesh.Clear();

            // Find required texture sizes.
            var textureWidth = TransitionTexture && TransitionTexture.width > MainTexture.width ? TransitionTexture.width : MainTexture.width;
            var textureHeight = TransitionTexture && TransitionTexture.height > MainTexture.height ? TransitionTexture.height : MainTexture.height;

            // Setup vertices.
            var quadHalfWidth = textureWidth * .5f / PixelsPerUnit;
            var quadHalfHeight = textureHeight * .5f / PixelsPerUnit;
            vertices[0] = new Vector3(-quadHalfWidth, -quadHalfHeight, 0);
            vertices[1] = new Vector3(-quadHalfWidth, quadHalfHeight, 0);
            vertices[2] = new Vector3(quadHalfWidth, -quadHalfHeight, 0);
            vertices[3] = new Vector3(quadHalfWidth, quadHalfHeight, 0);

            // Setup main texture UVs.
            var mainScaleRatioX = textureWidth / (float)MainTexture.width - 1;
            var mainScaleRatioY = textureHeight / (float)MainTexture.height - 1;
            var mainMaxX = 1 + mainScaleRatioX * (1 - Pivot.x);
            var mainMaxY = 1 + mainScaleRatioY * (1 - Pivot.y);
            var mainMinX = 0 - mainScaleRatioX * Pivot.x;
            var mainMinY = 0 - mainScaleRatioY * Pivot.y;
            mainUVs[0] = new Vector2(mainMinX, mainMinY);
            mainUVs[1] = new Vector2(mainMinX, mainMaxY);
            mainUVs[2] = new Vector2(mainMaxX, mainMinY);
            mainUVs[3] = new Vector2(mainMaxX, mainMaxY);

            if (TransitionTexture)
            {
                // Setup transition texture UVs.
                var transitionScaleRatioX = textureWidth / (float)TransitionTexture.width - 1;
                var transitionScaleRatioY = textureHeight / (float)TransitionTexture.height - 1;
                var transitionMaxX = 1 + transitionScaleRatioX * (1 - Pivot.x);
                var transitionMaxY = 1 + transitionScaleRatioY * (1 - Pivot.y);
                var transitionMinX = 0 - transitionScaleRatioX * Pivot.x;
                var transitionMinY = 0 - transitionScaleRatioY * Pivot.y;
                transitionUVs[0] = new Vector2(transitionMinX, transitionMinY);
                transitionUVs[1] = new Vector2(transitionMinX, transitionMaxY);
                transitionUVs[2] = new Vector2(transitionMaxX, transitionMinY);
                transitionUVs[3] = new Vector2(transitionMaxX, transitionMaxY);
            }

            // Apply pivot.
            UpdatePivot();

            // Create quad.
            meshFilter.mesh.SetVertices(vertices);
            meshFilter.mesh.SetUVs(0, mainUVs);
            meshFilter.mesh.SetUVs(1, transitionUVs);
            meshFilter.mesh.SetTriangles(triangles, 0);
        }

        /// <summary>
        /// Corrects geometry data to to match current pivot value.
        /// </summary>
        private void UpdatePivot ()
        {
            var spriteRect = EvaluateSpriteRect();

            var curPivot = new Vector2(-spriteRect.min.x / spriteRect.size.x, -spriteRect.min.y / spriteRect.size.y);
            if (curPivot == Pivot) return;

            var curDeltaX = spriteRect.size.x * curPivot.x;
            var curDeltaY = spriteRect.size.y * curPivot.y;
            var newDeltaX = spriteRect.size.x * Pivot.x;
            var newDeltaY = spriteRect.size.y * Pivot.y;

            var deltaPos = new Vector3(newDeltaX - curDeltaX, newDeltaY - curDeltaY);

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] -= deltaPos;
        }

        /// <summary>
        /// Calculates sprite rectangle using vertex data.
        /// </summary>
        private Rect EvaluateSpriteRect ()
        {
            var minVertPos = new Vector2(vertices.Min(v => v.x), vertices.Min(v => v.y));
            var maxVertPos = new Vector2(vertices.Max(v => v.x), vertices.Max(v => v.y));
            var spriteSizeX = Mathf.Abs(maxVertPos.x - minVertPos.x);
            var spriteSizeY = Mathf.Abs(maxVertPos.y - minVertPos.y);
            var spriteSize = new Vector2(spriteSizeX, spriteSizeY);
            return new Rect(minVertPos, spriteSize);
        }
    } 
}
