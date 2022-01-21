// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Naninovel.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CGGalleryPanel : CustomUI, ICGGalleryUI
    {
        public int CGCount => grid.SlotCount;

        protected string UnlockableIdPrefix => unlockableIdPrefix;
        protected ResourceLoaderConfiguration[] CGSources => cgSources;
        protected ScriptableButton ViewerPanel => viewerPanel;
        protected RawImage ViewerImage => viewerImage;
        protected CGGalleryGrid Grid => grid;

        [Header("CG Setup")]
        [Tooltip("All the unlockable item IDs with the specified prefix will be considered CG items.")]
        [SerializeField] private string unlockableIdPrefix = "CG";
        [Tooltip("The specified resource loaders will be used to retrieve the available CG slots and associated textures.")]
        [SerializeField] private ResourceLoaderConfiguration[] cgSources = {
            new ResourceLoaderConfiguration { PathPrefix = $"{UnlockablesConfiguration.DefaultPathPrefix}/CG" },
            new ResourceLoaderConfiguration { PathPrefix = $"{BackgroundsConfiguration.DefaultPathPrefix}/{BackgroundsConfiguration.MainActorId}/CG" }
        };
        [Tooltip("Whether to load only the resources required for the currently selected page and unload others. When disabled will preload all the CG resources on initialization and won't ever unload them.")]
        [SerializeField] private bool dynamicLoad = true;

        [Header("UI Setup")]
        [SerializeField] private ScriptableButton viewerPanel = default;
        [SerializeField] private RawImage viewerImage = default;
        [SerializeField] private CGGalleryGrid grid = default;

        private IUnlockableManager unlockableManager;
        private IResourceProviderManager providerManager;
        private ILocalizationManager localizationManager;
        private IInputManager inputManager;

        public override async UniTask InitializeAsync ()
        {
            foreach (var loaderConfig in cgSources)
            {
                // 1. Locate all the available textures under the source path.
                var loader = loaderConfig.CreateLocalizableFor<Texture2D>(providerManager, localizationManager);
                var resourcePaths = await loader.LocateAsync(string.Empty);
                // 2. Iterate the textures, adding them to the grid as CG slots.
                foreach (var resourcePath in resourcePaths)
                {
                    var unlockableId = $"{unlockableIdPrefix}/{resourcePath}";
                    if (grid.SlotExists(unlockableId)) continue;
                    var slot = new CGGalleryGridSlot.Constructor(grid.SlotPrototype, unlockableId, resourcePath, dynamicLoad, loader, HandleSlotClicked).ConstructedSlot;
                    grid.AddSlot(slot);
                }
            }

            if (!dynamicLoad) 
                await UniTask.WhenAll(grid.GetAllSlots().Select(s => s.LoadCGTextureAsync()));
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(grid, viewerPanel, viewerImage);

            unlockableManager = Engine.GetService<IUnlockableManager>();
            providerManager = Engine.GetService<IResourceProviderManager>();
            localizationManager = Engine.GetService<ILocalizationManager>();
            inputManager = Engine.GetService<IInputManager>();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            viewerPanel.OnButtonClicked += viewerPanel.Hide;

            if (inputManager?.GetCancel() != null)
                inputManager.GetCancel().OnStart += viewerPanel.Hide;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            viewerPanel.OnButtonClicked -= viewerPanel.Hide;

            if (inputManager?.GetCancel() != null)
                inputManager.GetCancel().OnStart -= viewerPanel.Hide;
        }
        
        protected virtual async void HandleSlotClicked (string id)
        {
            var slot = grid.GetSlot(id);
            if (!unlockableManager.ItemUnlocked(slot.UnlockableId)) return;

            var cgTexture = await slot.LoadCGTextureAsync();
            viewerImage.texture = cgTexture;
            viewerImage.SetMaterialDirty(); // Otherwise it won't show after closing CG panel and returning back (Unity regression).
            viewerPanel.Show();
        }
    }
}
