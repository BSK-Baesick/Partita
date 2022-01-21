// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IActor"/> implementation using <see cref="LayeredActorBehaviour"/> to represent the actor.
    /// </summary>
    public abstract class LayeredActor<TBehaviour, TMeta> : MonoBehaviourActor<TMeta> 
        where TBehaviour : LayeredActorBehaviour
        where TMeta : OrthoActorMetadata
    {
        public override string Appearance { get => appearance; set => SetAppearance(value); }
        public override bool Visible { get => visible; set => SetVisibility(value); }

        protected TransitionalRenderer TransitionalRenderer { get; private set; }
        protected TBehaviour Behaviour { get; private set; }

        private readonly Dictionary<object, HashSet<string>> heldAppearances = new Dictionary<object, HashSet<string>>();
        private LocalizableResourceLoader<GameObject> prefabLoader;
        private RenderTexture appearanceTexture;
        private string defaultAppearance;
        private string appearance;
        private bool visible;

        protected LayeredActor (string id, TMeta metadata)
            : base(id, metadata) { }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            
            if (ActorMetadata.RenderTexture)
            {
                ActorMetadata.RenderTexture.Clear();
                var textureRenderer = GameObject.AddComponent<TransitionalTextureRenderer>();
                textureRenderer.Initialize(ActorMetadata.CustomShader);
                textureRenderer.RenderTexture = ActorMetadata.RenderTexture;
                textureRenderer.CorrectAspect = ActorMetadata.CorrectRenderAspect;
                textureRenderer.DepthPassEnabled = ActorMetadata.EnableDepthPass;
                textureRenderer.DepthAlphaCutoff = ActorMetadata.DepthAlphaCutoff;
                TransitionalRenderer = textureRenderer;
            }
            else
            {
                var spriteRenderer = GameObject.AddComponent<TransitionalSpriteRenderer>();
                spriteRenderer.Initialize(ActorMetadata.Pivot, ActorMetadata.PixelsPerUnit, ActorMetadata.CustomShader);
                spriteRenderer.DepthPassEnabled = ActorMetadata.EnableDepthPass;
                spriteRenderer.DepthAlphaCutoff = ActorMetadata.DepthAlphaCutoff;
                TransitionalRenderer = spriteRenderer;
            }

            SetVisibility(false);

            var providerManager = Engine.GetService<IResourceProviderManager>();
            var localizationManager = Engine.GetService<ILocalizationManager>();
            prefabLoader = ActorMetadata.Loader.CreateLocalizableFor<GameObject>(providerManager, localizationManager);

            var prefabResource = await prefabLoader.LoadAsync(Id);
            Behaviour = Engine.Instantiate(prefabResource.Object).GetComponent<TBehaviour>();
            Behaviour.gameObject.name = prefabResource.Object.name;
            Behaviour.transform.SetParent(Transform);
            defaultAppearance = Behaviour.Composition;

            Engine.Behaviour.OnBehaviourUpdate += RenderAppearance;
        }

        public override async UniTask ChangeAppearanceAsync (string appearance, float duration, EasingType easingType = default,
            Transition? transition = default, CancellationToken cancellationToken = default)
        {
            this.appearance = appearance;

            if (string.IsNullOrEmpty(appearance))
                appearance = defaultAppearance;

            Behaviour.ApplyComposition(appearance);
            var previousTexture = appearanceTexture;
            appearanceTexture = Behaviour.Render(ActorMetadata.PixelsPerUnit);
            await TransitionalRenderer.TransitionToAsync(appearanceTexture, duration, easingType, transition, cancellationToken);
            if (cancellationToken.CancelASAP) return;

            // Release texture with the previous appearance.
            if (previousTexture)
                RenderTexture.ReleaseTemporary(previousTexture);
        }

        public override async UniTask ChangeVisibilityAsync (bool visible, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            // When revealing the actor and never rendered before — force render with default appearance.
            if (!Visible && visible && string.IsNullOrEmpty(appearance))
                SetAppearance(defaultAppearance);

            this.visible = visible;

            await TransitionalRenderer.FadeToAsync(visible ? TintColor.a : 0, duration, easingType, cancellationToken);
        }

        public override async UniTask HoldResourcesAsync (string appearance, object holder)
        {
            if (!heldAppearances.ContainsKey(holder))
            {
                await prefabLoader.LoadAndHoldAsync(Id, holder);
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
                prefabLoader?.Release(Id, holder);
            }
        }

        public override void Dispose ()
        {
            if (Engine.Behaviour != null)
                Engine.Behaviour.OnBehaviourUpdate -= RenderAppearance;

            if (appearanceTexture)
                RenderTexture.ReleaseTemporary(appearanceTexture);

            base.Dispose();

            prefabLoader?.UnloadAll();
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

        protected virtual void RenderAppearance ()
        {
            if (!Behaviour || !Behaviour.Animated || !appearanceTexture) return;

            Behaviour.Render(ActorMetadata.PixelsPerUnit, appearanceTexture);
        }
    }
}
