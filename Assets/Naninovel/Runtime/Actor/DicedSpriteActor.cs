// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

#if SPRITE_DICING_AVAILABLE

using System;
using System.Collections.Generic;
using System.Linq;
using SpriteDicing;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="MonoBehaviourActor{TMeta}"/> using "SpriteDicing" extension to represent the actor.
    /// </summary>
    public abstract class DicedSpriteActor<TMeta> : MonoBehaviourActor<TMeta>
        where TMeta : OrthoActorMetadata
    {
        public override string Appearance { get => appearance; set => SetAppearance(value); }
        public override bool Visible { get => visible; set => SetVisibility(value); }

        protected virtual TransitionalRenderer TransitionalRenderer { get; private set; }

        private readonly OrthoActorMetadata metadata;
        private readonly Material dicedMaterial;
        private readonly Mesh dicedMesh;
        private readonly Dictionary<object, HashSet<string>> heldAppearances = new Dictionary<object, HashSet<string>>();
        private LocalizableResourceLoader<DicedSpriteAtlas> atlasLoader;
        private RenderTexture appearanceTexture;
        private string appearance;
        private string defaultSpriteName;
        private bool visible;

        protected DicedSpriteActor (string id, TMeta metadata)
            : base(id, metadata)
        {
            this.metadata = metadata;

            dicedMaterial = new Material(Shader.Find("Sprites/Default"));
            dicedMaterial.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

            dicedMesh = new Mesh();
            dicedMesh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            dicedMesh.name = $"{id} Diced Sprite Mesh";
        }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            
            if (metadata.RenderTexture)
            {
                metadata.RenderTexture.Clear();
                var textureRenderer = GameObject.AddComponent<TransitionalTextureRenderer>();
                textureRenderer.Initialize(metadata.CustomShader);
                textureRenderer.RenderTexture = metadata.RenderTexture;
                textureRenderer.CorrectAspect = metadata.CorrectRenderAspect;
                textureRenderer.DepthPassEnabled = metadata.EnableDepthPass;
                textureRenderer.DepthAlphaCutoff = metadata.DepthAlphaCutoff;
                TransitionalRenderer = textureRenderer;
            }
            else
            {
                var spriteRenderer = GameObject.AddComponent<TransitionalSpriteRenderer>();
                spriteRenderer.Initialize(metadata.Pivot, metadata.PixelsPerUnit, metadata.CustomShader);
                spriteRenderer.DepthPassEnabled = metadata.EnableDepthPass;
                spriteRenderer.DepthAlphaCutoff = metadata.DepthAlphaCutoff;
                TransitionalRenderer = spriteRenderer;
            }
            
            SetVisibility(false);

            var providerManager = Engine.GetService<IResourceProviderManager>();
            var localizationManager = Engine.GetService<ILocalizationManager>();
            atlasLoader = metadata.Loader.CreateLocalizableFor<DicedSpriteAtlas>(providerManager, localizationManager);
        }

        public override async UniTask ChangeAppearanceAsync (string appearance, float duration, EasingType easingType = default,
            Transition? transition = default, CancellationToken cancellationToken = default)
        {
            var atlasResource = await atlasLoader.LoadAsync(Id);
            if (cancellationToken.CancelASAP) return;
            if (!atlasResource.Valid || atlasResource.Object.SpritesCount == 0) return;

            if (string.IsNullOrEmpty(defaultSpriteName))
            {
                var sprites = atlasResource.Object.GetAllSprites();
                var defaultSprite = sprites.Find(s => s.name.EndsWithFast("Default"));
                defaultSpriteName = ObjectUtils.IsValid(defaultSprite) ? defaultSprite.name : sprites.First().name;
            }

            if (string.IsNullOrEmpty(appearance))
                appearance = defaultSpriteName;

            this.appearance = appearance;

            // In case user stored source sprites in folders, the diced sprites will have dots in their names.
            var spriteName = appearance.Replace("/", ".");
            var dicedSprite = atlasResource.Object.GetSprite(spriteName);
            if (dicedSprite is null)
            {
                Debug.LogWarning($"Failed to get `{spriteName}` diced sprite from `{atlasResource.Object.name}` atlas to set `{appearance}` appearance for `{Id}` character.");
                return;
            }
            dicedMesh.vertices = Array.ConvertAll(dicedSprite.vertices, i => new Vector3(i.x, i.y));
            dicedMesh.uv = dicedSprite.uv;
            dicedMesh.triangles = Array.ConvertAll(dicedSprite.triangles, i => (int)i);
            dicedMaterial.mainTexture = dicedSprite.texture;

            // Create a texture with the new appearance.
            var spriteRect = dicedSprite.GetVerticesRect();
            var newRenderTexture = RenderTexture.GetTemporary(Mathf.CeilToInt(spriteRect.width * metadata.PixelsPerUnit), Mathf.CeilToInt(spriteRect.height * metadata.PixelsPerUnit));
            Graphics.SetRenderTarget(newRenderTexture);
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            var halfRectSize = spriteRect.size / 2f;
            GL.LoadProjectionMatrix(Matrix4x4.Ortho(-halfRectSize.x, halfRectSize.x, -halfRectSize.y, halfRectSize.y, 0f, 100f));
            dicedMaterial.SetPass(0);
            var drawPos = new Vector3(spriteRect.width * dicedSprite.pivot.x - halfRectSize.x, spriteRect.height * dicedSprite.pivot.y - halfRectSize.y);
            Graphics.DrawMeshNow(dicedMesh, drawPos, Quaternion.identity);
            GL.PopMatrix();

            await TransitionalRenderer.TransitionToAsync(newRenderTexture, duration, easingType, transition, cancellationToken);
            if (cancellationToken.CancelASAP) return;

            // Release texture with the old appearance.
            if (ObjectUtils.IsValid(appearanceTexture))
                RenderTexture.ReleaseTemporary(appearanceTexture);
            appearanceTexture = newRenderTexture;
        }

        public override async UniTask ChangeVisibilityAsync (bool visible, float duration, 
            EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            // When appearance is not set (and default one is not preloaded for some reason, eg when using dynamic parameters) 
            // and revealing the actor — attempt to set default appearance.
            if (!Visible && visible && string.IsNullOrWhiteSpace(Appearance))
                await ChangeAppearanceAsync(defaultSpriteName, 0, cancellationToken: cancellationToken);

            this.visible = visible;

            await TransitionalRenderer.FadeToAsync(visible ? TintColor.a : 0, duration, easingType, cancellationToken);
        }

        public override async UniTask HoldResourcesAsync (string appearance, object holder)
        {
            if (!heldAppearances.ContainsKey(holder))
            {
                await atlasLoader.LoadAndHoldAsync(Id, holder);
                heldAppearances.Add(holder, new HashSet<string>());
            }

            heldAppearances[holder].Add(appearance);
        }

        public override void ReleaseResources (string appearance, object holder)
        {
            if (!heldAppearances.ContainsKey(holder)) return;
            
            heldAppearances[holder].Remove(appearance);
            if (heldAppearances.Count == 0)
            {
                heldAppearances.Remove(holder);
                atlasLoader?.Release(Id, holder);
            }
        }

        public override void Dispose ()
        {
            if (ObjectUtils.IsValid(appearanceTexture))
                RenderTexture.ReleaseTemporary(appearanceTexture);

            base.Dispose();

            atlasLoader?.UnloadAll();
        }

        protected virtual void SetAppearance (string appearance) => ChangeAppearanceAsync(appearance, 0).Forget();

        protected virtual void SetVisibility (bool visible) => ChangeVisibilityAsync(visible, 0).Forget();

        protected override Color GetBehaviourTintColor () => TransitionalRenderer.TintColor;

        protected override void SetBehaviourTintColor (Color tintColor)
        {
            if (!Visible) // Handle visibility-controlled alpha of the tint color.
                tintColor.a = TransitionalRenderer.TintColor.a;
            TransitionalRenderer.TintColor = tintColor;
        }
    }
}

#endif
