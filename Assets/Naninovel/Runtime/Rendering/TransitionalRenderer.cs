// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Allows rendering a texture with <see cref="TransitionalMaterial"/> and transition to another texture with a set of configurable visual effects.
    /// </summary>
    public abstract class TransitionalRenderer : MonoBehaviour
    {
        /// <summary>
        /// Current transition mode data.
        /// </summary>
        public virtual Transition Transition
        {
            get => new Transition(TransitionName, TransitionParams, DissolveTexture);
            set { TransitionName = value.Name; TransitionParams = value.Parameters; DissolveTexture = value.DissolveTexture; }
        }
        /// <inheritdoc cref="TransitionalMaterial.MainTexture"/>
        public virtual Texture MainTexture { get => Material.MainTexture; set => Material.MainTexture = DepthMaterial.MainTexture = value; }
        /// <inheritdoc cref="TransitionalMaterial.TransitionTexture"/>
        public virtual Texture TransitionTexture { get => Material.TransitionTexture; set { Material.TransitionTexture = value; DepthMaterial.TransitionTexture = value; } }
        /// <inheritdoc cref="TransitionalMaterial.DissolveTexture"/>
        public virtual Texture DissolveTexture { get => Material.DissolveTexture; set { Material.DissolveTexture = value; DepthMaterial.DissolveTexture = value; } }
        /// <inheritdoc cref="TransitionalMaterial.TransitionName"/>
        public virtual string TransitionName { get => Material.TransitionName; set { Material.TransitionName = value; DepthMaterial.TransitionName = value; } }
        /// <inheritdoc cref="TransitionalMaterial.TransitionProgress"/>
        public virtual float TransitionProgress { get => Material.TransitionProgress; set { Material.TransitionProgress = value; DepthMaterial.TransitionProgress = value; } }
        /// <inheritdoc cref="TransitionalMaterial.TransitionParams"/>
        public virtual Vector4 TransitionParams { get => Material.TransitionParams; set { Material.TransitionParams = value; DepthMaterial.TransitionParams = value; } }
        /// <inheritdoc cref="TransitionalMaterial.RandomSeed"/>
        public virtual Vector2 RandomSeed { get => Material.RandomSeed; set { Material.RandomSeed = value; DepthMaterial.RandomSeed = value; } }
        /// <inheritdoc cref="TransitionalMaterial.TintColor"/>
        public virtual Color TintColor { get => Material.TintColor; set { Material.TintColor = value; DepthMaterial.TintColor = value; } }
        /// <inheritdoc cref="TransitionalMaterial.Opacity"/>
        public virtual float Opacity { get => Material.Opacity; set { Material.Opacity = value; DepthMaterial.Opacity = value; } }
        /// <inheritdoc cref="TransitionalMaterial.FlipX"/>
        public virtual bool FlipX { get => Material.FlipX; set { Material.FlipX = value; DepthMaterial.FlipX = value; } }
        /// <inheritdoc cref="TransitionalMaterial.FlipY"/>
        public virtual bool FlipY { get => Material.FlipY; set { Material.FlipY = value; DepthMaterial.FlipY = value; } }
        /// <inheritdoc cref="TransitionalMaterial.DepthPassEnabled"/>
        public virtual bool DepthPassEnabled { get => DepthMaterial.DepthPassEnabled; set => DepthMaterial.DepthPassEnabled = value; }
        /// <inheritdoc cref="TransitionalMaterial.DepthAlphaCutoff"/>
        public virtual float DepthAlphaCutoff { get => DepthMaterial.DepthAlphaCutoff; set => DepthMaterial.DepthAlphaCutoff = value; }

        protected abstract string DefaultShaderName { get; }
        protected virtual TransitionalMaterial Material { get; private set; }
        protected virtual TransitionalMaterial DepthMaterial { get; private set; }
        
        private readonly Tweener<FloatTween> transitionTweener = new Tweener<FloatTween>();
        private readonly Tweener<ColorTween> colorTweener = new Tweener<ColorTween>();
        private readonly Tweener<FloatTween> fadeTweener = new Tweener<FloatTween>();

        /// <summary>
        /// Prepares the underlying systems for render.
        /// </summary>
        /// <param name="customShader">Shader to use for rendering; will use a default one when not provided.</param>
        public virtual void Initialize (Shader customShader = default)
        {
            Material = new TransitionalMaterial(TransitionalMaterial.Variant.Default, customShader ? customShader : Shader.Find(DefaultShaderName));
            DepthMaterial = new TransitionalMaterial(TransitionalMaterial.Variant.Depth, customShader ? customShader : Shader.Find(DefaultShaderName));
        }

        /// <inheritdoc cref="TransitionalMaterial.UpdateRandomSeed"/>
        public virtual void UpdateRandomSeed ()
        {
            Material.UpdateRandomSeed();
            DepthMaterial.UpdateRandomSeed();
        }

        /// <summary>
        /// Performs transition from <see cref="MainTexture"/> to the provided <paramref name="texture"/> over <paramref name="duration"/>.
        /// </summary>
        /// <param name="texture">Texture to transition into.</param>
        /// <param name="duration">Duration of the transition, in seconds.</param>
        /// <param name="easingType">Type of easing to use when applying the transition effect.</param>
        /// <param name="transition">Type of the transition effect to use.</param>
        public virtual async UniTask TransitionToAsync (Texture texture, float duration, EasingType easingType = default,
            Transition? transition = default, CancellationToken cancellationToken = default)
        {
            if (transitionTweener.Running)
            {
                transitionTweener.CompleteInstantly();
                await AsyncUtils.WaitEndOfFrame; // Materials are updated later in render loop, so wait before further modifications.
                if (cancellationToken.CancelASAP) return;
            }

            if (transition.HasValue)
                Transition = transition.Value;

            if (duration <= 0)
            {
                MainTexture = texture;
                TransitionProgress = 0;
                return;
            }
            else
            {
                if (!MainTexture) MainTexture = texture;

                UpdateRandomSeed();
                TransitionTexture = texture;
                var tween = new FloatTween(TransitionProgress, 1, duration, value => TransitionProgress = value, false, easingType, this);
                await transitionTweener.RunAsync(tween, cancellationToken);
                if (cancellationToken.CancelASAP) return;
                MainTexture = TransitionTexture;
                TransitionProgress = 0;
            }
        }

        /// <summary>
        /// Tints current texture to the provided <param name="color"></param> over <paramref name="duration"/>.
        /// </summary>
        /// <param name="color">Color of the tint.</param>
        /// <param name="duration">Duration of crossfade from current to the target tint color.</param>
        /// <param name="easingType">Type of easing to use when applying the tint.</param>
        public virtual async UniTask TintToAsync (Color color, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            if (colorTweener.Running) colorTweener.CompleteInstantly();

            if (duration <= 0)
            {
                TintColor = color;
                return;
            }

            var tween = new ColorTween(TintColor, color, ColorTweenMode.All, duration, value => TintColor = value, false, easingType, this);
            await colorTweener.RunAsync(tween, cancellationToken);
        }

        /// <summary>
        /// Same as tint, but applies only to the alpha component of the color.
        /// </summary>
        public virtual async UniTask FadeToAsync (float opacity, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            if (fadeTweener.Running) fadeTweener.CompleteInstantly();

            if (duration <= 0)
            {
                Opacity = opacity;
                return;
            }

            var tween = new FloatTween(Opacity, opacity, duration, value => Opacity = value, false, easingType, this);
            await fadeTweener.RunAsync(tween, cancellationToken);
        }

        public virtual async UniTask FadeOutAsync (float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            await FadeToAsync(0, duration, easingType, cancellationToken);
        }

        public virtual async UniTask FadeInAsync (float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            await FadeToAsync(1, duration, easingType, cancellationToken);   
        }
    } 
}
