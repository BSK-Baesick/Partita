// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;
using UnityEngine.Rendering;

namespace Naninovel
{
    public class TransitionalMaterial : Material
    {
        public enum Variant { Default, Depth }

        public static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        public static readonly int TransitionTexId = Shader.PropertyToID("_TransitionTex");
        public static readonly int CloudsTexId = Shader.PropertyToID("_CloudsTex");
        public static readonly int DissolveTexId = Shader.PropertyToID("_DissolveTex");
        public static readonly int TransitionProgressId = Shader.PropertyToID("_TransitionProgress");
        public static readonly int TransitionParamsId = Shader.PropertyToID("_TransitionParams");
        public static readonly int RandomSeedId = Shader.PropertyToID("_RandomSeed");
        public static readonly int TintColorId = Shader.PropertyToID("_TintColor");
        public static readonly int FlipId = Shader.PropertyToID("_Flip");
        public static readonly int DepthAlphaCutoffId = Shader.PropertyToID("_DepthAlphaCutoff");
        public const string DefaultShaderName = "Naninovel/TransitionalSprite";
        public const string TransparentPassName = "Transparent";
        public const string DepthMaskPassName = "DepthMask";

        /// <summary>
        /// Current main texture.
        /// </summary>
        public Texture MainTexture { get => mainTexture; set => mainTexture = value; }
        /// <summary>
        /// Current texture that is used to transition from <see cref="MainTexture"/>.
        /// </summary>
        public Texture TransitionTexture { get => GetTexture(TransitionTexId); set => SetTexture(TransitionTexId, value); }
        /// <summary>
        /// Texture used in a custom dissolve transition type.
        /// </summary>
        public Texture DissolveTexture { get => GetTexture(DissolveTexId); set => SetTexture(DissolveTexId, value); }
        /// <summary>
        /// Name of the current transition type.
        /// </summary>
        public string TransitionName { get => TransitionUtils.GetEnabled(this); set => TransitionUtils.EnableKeyword(this, value); }
        /// <summary>
        /// Current transition progress between <see cref="MainTexture"/> and <see cref="TransitionTexture"/>, in 0.0 to 1.0 range.
        /// </summary>
        public float TransitionProgress { get => GetFloat(TransitionProgressId); set => SetFloat(TransitionProgressId, value); }
        /// <summary>
        /// Parameters of the current transition.
        /// </summary>
        public Vector4 TransitionParams { get => GetVector(TransitionParamsId); set => SetVector(TransitionParamsId, value); }
        /// <summary>
        /// Current random seed used in some transition types.
        /// </summary>
        public Vector2 RandomSeed { get => GetVector(RandomSeedId); set => SetVector(RandomSeedId, value); }
        /// <summary>
        /// Current tint color.
        /// </summary>
        public Color TintColor { get => GetColor(TintColorId); set => SetColor(TintColorId, value); }
        /// <summary>
        /// Current alpha component of <see cref="TintColor"/>.
        /// </summary>
        public float Opacity { get => GetColor(TintColorId).a; set => SetOpacity(value); }
        /// <summary>
        /// Whether the render result should be flipped by x-axis.
        /// </summary>
        public bool FlipX { get => Mathf.Approximately(GetVector(FlipId).x, -1); set => SetFlipX(value); }
        /// <summary>
        /// Whether the render result should be flipped by y-axis.
        /// </summary>
        public bool FlipY { get => Mathf.Approximately(GetVector(FlipId).y, -1); set => SetFlipY(value); }
        /// <summary>
        /// Whether an additional depth pass should be performed.
        /// </summary>
        public bool DepthPassEnabled { get => GetShaderPassEnabled(DepthMaskPassName); set => SetShaderPassEnabled(DepthMaskPassName, value); }
        /// <summary>
        /// Whether to discard semi-transparent pixels when performing depth pass.
        /// </summary>
        public float DepthAlphaCutoff { get => GetFloat(DepthAlphaCutoffId); set => SetFloat(DepthAlphaCutoffId, Mathf.Clamp(value, 0.01f, 0.99f)); }

        private static Texture2D sharedCloudsTexture;

        public TransitionalMaterial (Variant variant, Shader customShader = default, HideFlags hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor)
            : base(customShader ? customShader : Shader.Find(DefaultShaderName)) 
        {
            if (!sharedCloudsTexture)
                sharedCloudsTexture = Resources.Load<Texture2D>("Naninovel/Textures/Clouds");

            switch (variant)
            {
                case Variant.Default:
                    renderQueue = (int)RenderQueue.Transparent;
                    SetShaderPassEnabled(TransparentPassName, true);
                    SetShaderPassEnabled(DepthMaskPassName, false);
                    SetOverrideTag("RenderType", "Transparent");
                    break;
                case Variant.Depth:
                    renderQueue = (int)RenderQueue.AlphaTest;
                    SetShaderPassEnabled(TransparentPassName, false);
                    SetShaderPassEnabled(DepthMaskPassName, true);
                    SetOverrideTag("RenderType", "TransparentCutout");
                    break;
            }

            SetTexture(CloudsTexId, sharedCloudsTexture);
            this.hideFlags = hideFlags;
        }

        /// <summary>
        /// Regenerate current value of <see cref="RandomSeed"/>.
        /// </summary>
        public void UpdateRandomSeed ()
        {
            var sinTime = Mathf.Sin(Time.time);
            var cosTime = Mathf.Cos(Time.time);
            RandomSeed = new Vector2(Mathf.Abs(sinTime), Mathf.Abs(cosTime));
        }

        private void SetOpacity (float value)
        {
            var color = TintColor;
            color.a = value;
            TintColor = color;
        }

        private void SetFlipX (bool value)
        {
            var flip = GetVector(FlipId);
            flip.x = value ? -1 : 1;
            SetVector(FlipId, flip);
        }

        private void SetFlipY (bool value)
        {
            var flip = GetVector(FlipId);
            flip.y = value ? -1 : 1;
            SetVector(FlipId, flip);
        }
    }
}
