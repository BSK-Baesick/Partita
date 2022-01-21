// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Naninovel
{
    public class CGGalleryGridSlot : ScriptableGridSlot
    {
        public class Constructor : Constructor<CGGalleryGridSlot>
        {
            public Constructor (CGGalleryGridSlot prototype, string unlockableId, string textureLocalPath, bool unloadOnDisable, 
                LocalizableResourceLoader<Texture2D> cgTextureLoader, OnClicked onClicked) : base(prototype, unlockableId, onClicked)
            {
                ConstructedSlot.textureLoader = cgTextureLoader;
                ConstructedSlot.textureLocalPath = textureLocalPath;
                ConstructedSlot.thumbnailImage.texture = ConstructedSlot.loadingTexture;
                ConstructedSlot.unloadOnDisable = unloadOnDisable;
            }
        }

        public virtual string UnlockableId => Id;

        protected RawImage ThumbnailImage => thumbnailImage;
        protected Texture2D LockedTexture => lockedTexture;
        protected Texture2D LoadingTexture => loadingTexture;

        [SerializeField] private RawImage thumbnailImage = null;
        [SerializeField] private Texture2D lockedTexture = default;
        [SerializeField] private Texture2D loadingTexture = default;

        private string textureLocalPath;
        private bool unloadOnDisable;
        private IUnlockableManager unlockableManager;
        private LocalizableResourceLoader<Texture2D> textureLoader;

        public virtual async UniTask<Texture2D> LoadCGTextureAsync ()
        {
            Texture2D cgTexture;

            if (textureLoader.IsLoaded(textureLocalPath))
                cgTexture = textureLoader.GetLoadedOrNull(textureLocalPath);
            else
            {
                thumbnailImage.texture = loadingTexture;
                var resource = await textureLoader.LoadAndHoldAsync(textureLocalPath, this);
                cgTexture = resource;
            }

            return cgTexture;
        }

        public virtual void UnloadCGTexture ()
        {
            textureLoader?.Release(textureLocalPath, this);
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(thumbnailImage, lockedTexture);

            unlockableManager = Engine.GetService<IUnlockableManager>();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();
            unlockableManager.OnItemUpdated += HandleItemUpdated;
            HandleItemUpdated(null);
        }

        protected override void OnDisable ()
        {
            base.OnDisable();
            unlockableManager.OnItemUpdated -= HandleItemUpdated;
            if (unloadOnDisable) UnloadCGTexture();
        }

        protected virtual async void HandleItemUpdated (UnlockableItemUpdatedArgs _)
        {
            while (UnlockableId is null) // We get here after first OnEnable, but ID is not set yet.
            {
                await UniTask.DelayFrame(1);
                if (!gameObject) return;
            }

            if (!unlockableManager.ItemUnlocked(UnlockableId)) thumbnailImage.texture = lockedTexture;
            else thumbnailImage.texture = await LoadCGTextureAsync();
        }
    }
}
