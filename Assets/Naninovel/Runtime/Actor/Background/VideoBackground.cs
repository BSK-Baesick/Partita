// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Video;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IBackgroundActor"/> implementation using <see cref="VideoClip"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(VideoClip), true)]
    public class VideoBackground : MonoBehaviourActor<BackgroundMetadata>, IBackgroundActor
    {
        private class VideoData { public VideoPlayer Player; public RenderTexture RenderTexture; }

        public override string Appearance { get => appearance; set => SetAppearance(value); }
        public override bool Visible { get => visible; set => SetVisibility(value); }

        protected virtual TransitionalRenderer TransitionalRenderer { get; private set; }

        private static bool sharedResourcesInitialized;
        private static int sharedRefCounter;
        private static RenderTextureDescriptor renderTextureDescriptor;
        private static LiteralMap<VideoData> videoDataMap;
        private static Vector2Int referenceResolution;

        private string appearance;
        private bool visible;
        private LocalizableResourceLoader<VideoClip> videoLoader;
        // ReSharper disable once NotAccessedField.Local (Used in WebGL pragma)
        private string streamExtension;

        public VideoBackground (string id, BackgroundMetadata metadata)
            : base(id, metadata)
        {
            if (referenceResolution == default)
                referenceResolution = Engine.GetConfiguration<CameraConfiguration>().ReferenceResolution;

            streamExtension = Engine.GetConfiguration<ResourceProviderConfiguration>().VideoStreamExtension;

            InitializeSharedResources();
            sharedRefCounter++;

            var providerManager = Engine.GetService<IResourceProviderManager>();
            var localizationManager = Engine.GetService<ILocalizationManager>();
            videoLoader = new LocalizableResourceLoader<VideoClip>(
                providerManager.GetProviders(metadata.Loader.ProviderTypes), 
                localizationManager, $"{metadata.Loader.PathPrefix}/{id}");
        }

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
        }

        public override async UniTask ChangeAppearanceAsync (string appearance, float duration, EasingType easingType = default,
            Transition? transition = default, CancellationToken cancellationToken = default)
        {
            this.appearance = appearance;

            if (string.IsNullOrEmpty(appearance)) return;

            var videoData = await GetOrLoadVideoDataAsync(appearance);
            if (cancellationToken.CancelASAP) return;
            if (!videoData.Player.isPrepared)
            {
                videoData.Player.Prepare();
                while (!videoData.Player.isPrepared) 
                    await AsyncUtils.WaitEndOfFrame;
                if (cancellationToken.CancelASAP) return;
            }
            videoData.Player.Play();

            await TransitionalRenderer.TransitionToAsync(videoData.RenderTexture, duration, easingType, transition, cancellationToken);
            if (cancellationToken.CancelASAP) return;

            foreach (var kv in videoDataMap) // Make sure no other videos are playing.
                if (!kv.Key.EqualsFast(appearance))
                    kv.Value.Player.Stop();
        }

        public override async UniTask ChangeVisibilityAsync (bool isVisible, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            this.visible = isVisible;

            await TransitionalRenderer.FadeToAsync(isVisible ? 1 : 0, duration, easingType, cancellationToken);
        }

        public override async UniTask HoldResourcesAsync (string appearance, object holder)
        {
            if (string.IsNullOrEmpty(appearance)) return;

            await GetOrLoadVideoDataAsync(appearance);

            // Releasing is done in Dispose().
            videoLoader.Hold(appearance, this);
        }

        public override void Dispose ()
        {
            base.Dispose();

            videoLoader?.ReleaseAll(this);
            sharedRefCounter--;
            DestroySharedResources();
        }

        protected virtual void SetAppearance (string appearance) => ChangeAppearanceAsync(appearance, 0).Forget();

        protected virtual void SetVisibility (bool visible) => ChangeVisibilityAsync(visible, 0).Forget();

        protected override Color GetBehaviourTintColor () => Color.white;

        protected override void SetBehaviourTintColor (Color tintColor) { }

        private async UniTask<VideoData> GetOrLoadVideoDataAsync (string videoName)
        {
            if (videoDataMap.ContainsKey(videoName)) return videoDataMap[videoName];

            var renderTexture = new RenderTexture(renderTextureDescriptor);
            var videoPlayer = Engine.CreateObject<VideoPlayer>(videoName);
            videoPlayer.playOnAwake = false;

            #if UNITY_WEBGL && !UNITY_EDITOR
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = PathUtils.Combine(Application.streamingAssetsPath, $"{ActorMetadata.Loader.PathPrefix}/{Id}/{videoName}") + streamExtension;
            await AsyncUtils.WaitEndOfFrame;
            #else
            var videoClip = await videoLoader.LoadAsync(videoName);
            if (!videoClip.Valid) throw new Exception($"Failed to load `{videoName}` resource for `{Id}` video background actor. Make sure the video clip is assigned in the actor resources.");
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            #endif

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.isLooping = true;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

            var sceneData = new VideoData { Player = videoPlayer, RenderTexture = renderTexture };
            videoDataMap[videoName] = sceneData;

            return sceneData;
        }

        private static void InitializeSharedResources ()
        {
            if (sharedResourcesInitialized) return;

            renderTextureDescriptor = new RenderTextureDescriptor(referenceResolution.x, referenceResolution.y, RenderTextureFormat.Default);
            videoDataMap = new LiteralMap<VideoData>();
            sharedResourcesInitialized = true;
        }

        private static void DestroySharedResources ()
        {
            if (sharedRefCounter > 0) return;

            foreach (var videoData in videoDataMap.Values)
            {
                videoData.Player.Stop();
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(videoData.Player.gameObject);
                    UnityEngine.Object.Destroy(videoData.RenderTexture);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(videoData.Player.gameObject);
                    UnityEngine.Object.DestroyImmediate(videoData.RenderTexture);
                }
            }

            sharedResourcesInitialized = false;
        }
    }
}
