// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Represents a layer inside <see cref="LayeredActorBehaviour"/> object.
    /// </summary>
    public class LayeredActorLayer
    {
        public readonly string Name;
        public readonly string Group = string.Empty;
        public readonly Mesh Mesh;
        public readonly Material RenderMaterial;
        public bool Enabled { get => renderer.enabled; set => renderer.enabled = value; }
        public Vector2 Position => renderer.transform.position;
        public Quaternion Rotation => renderer.transform.localRotation;
        public Vector2 Scale => renderer.transform.lossyScale;
        public Texture Texture => renderer is SpriteRenderer sr ? sr.sprite.texture : renderer.sharedMaterial.mainTexture;
        public Color Color => renderer is SpriteRenderer sr ? sr.color :
            (renderer.sharedMaterial.HasProperty(colorId) ? renderer.sharedMaterial.color : Color.white);

        private static readonly int colorId = Shader.PropertyToID("_Color");

        private readonly Renderer renderer;

        public LayeredActorLayer (Renderer renderer, Mesh mesh)
        {
            this.renderer = renderer;
            Mesh = mesh;
            Name = renderer.gameObject.name;

            if (Application.isPlaying)
            {
                renderer.forceRenderingOff = true;
                RenderMaterial = renderer.material;
                RenderMaterial.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }

            var transform = renderer.transform.parent;
            while (transform && !transform.TryGetComponent<LayeredActorBehaviour>(out _))
            {
                Group = transform.name + (string.IsNullOrEmpty(Group) ? string.Empty : $"/{Group}");
                transform = transform.parent;
            }
        }

        public LayeredActorLayer (SpriteRenderer spriteRenderer) :
            this(spriteRenderer, BuildSpriteMesh(spriteRenderer))
        {
            if (Application.isPlaying)
                spriteRenderer.material.mainTexture = spriteRenderer.sprite.texture;
        }

        private static Mesh BuildSpriteMesh (SpriteRenderer spriteRenderer)
        {
            var sprite = spriteRenderer.sprite;
            var mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            mesh.name = $"{sprite.name} Sprite Mesh";
            mesh.vertices = Array.ConvertAll(sprite.vertices, i => new Vector3(i.x * (spriteRenderer.flipX ? -1 : 1), i.y * (spriteRenderer.flipY ? -1 : 1)));
            mesh.uv = sprite.uv;
            mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);
            return mesh;
        }
    }
}
