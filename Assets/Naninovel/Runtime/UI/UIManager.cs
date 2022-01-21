// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using Naninovel.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <inheritdoc cref="IUIManager"/>
    [InitializeAtRuntime]
    public class UIManager : IUIManager, IStatefulService<SettingsStateMap>
    {
        [Serializable]
        public class Settings
        {
            public string FontName = default;
            public int FontSize = -1;
        }

        private readonly struct ManagedUI
        {
            public readonly string Name;
            public readonly GameObject GameObject;
            public readonly IManagedUI UIComponent;
            public readonly Type ComponentType;

            public ManagedUI (string name, GameObject gameObject, IManagedUI uiComponent)
            {
                Name = name;
                GameObject = gameObject;
                UIComponent = uiComponent;
                ComponentType = UIComponent?.GetType();
            }
        }

        public virtual UIConfiguration Configuration { get; }
        public virtual string FontName { get => fontName; set => SetFontName(value); }
        public virtual int FontSize { get => fontSize; set => SetFontSize(value); }

        private readonly List<ManagedUI> managedUI = new List<ManagedUI>();
        private readonly Dictionary<Type, IManagedUI> cachedGetUIResults = new Dictionary<Type, IManagedUI>();
        private readonly Dictionary<IManagedUI, bool> modalState = new Dictionary<IManagedUI, bool>();
        private readonly ICameraManager cameraManager;
        private readonly IInputManager inputManager;
        private readonly IResourceProviderManager providersManager;
        private ResourceLoader<GameObject> loader;
        private Camera customCamera;
        private IInputSampler toggleUIInput;
        private string fontName;
        private int fontSize = -1;

        public UIManager (UIConfiguration config, IResourceProviderManager providersManager, ICameraManager cameraManager, IInputManager inputManager)
        {
            Configuration = config;
            this.providersManager = providersManager;
            this.cameraManager = cameraManager;
            this.inputManager = inputManager;

            // Instantiating the UIs after the engine initialization so that UIs can use Engine API in Awake() and OnEnable() methods.
            Engine.AddPostInitializationTask(InstantiateUIsAsync);
        }

        public virtual UniTask InitializeServiceAsync ()
        {
            loader = Configuration.Loader.CreateFor<GameObject>(providersManager);

            toggleUIInput = inputManager.GetToggleUI();
            if (toggleUIInput != null)
                toggleUIInput.OnStart += ToggleUI;

            return UniTask.CompletedTask;
        }

        public virtual void ResetService () { }

        public virtual void DestroyService ()
        {
            if (toggleUIInput != null)
                toggleUIInput.OnStart -= ToggleUI;

            foreach (var ui in managedUI)
                ObjectUtils.DestroyOrImmediate(ui.GameObject);
            managedUI.Clear();
            cachedGetUIResults.Clear();

            loader?.UnloadAll();

            Engine.RemovePostInitializationTask(InstantiateUIsAsync);
        }

        public virtual void SaveServiceState (SettingsStateMap stateMap)
        {
            var settings = new Settings {
                FontName = FontName,
                FontSize = FontSize
            };
            stateMap.SetState(settings);
        }

        public virtual UniTask LoadServiceStateAsync (SettingsStateMap stateMap)
        {
            var settings = stateMap.GetState<Settings>() ?? new Settings();
            FontName = settings.FontName;
            FontSize = settings.FontSize;

            return UniTask.CompletedTask;
        }

        public virtual async UniTask<IManagedUI> InstantiatePrefabAsync (GameObject prefab, string name = default)
        {
            var gameObject = Engine.Instantiate(prefab, prefab.name, Configuration.OverrideObjectsLayer ? (int?)Configuration.ObjectsLayer : null);

            if (!gameObject.TryGetComponent<IManagedUI>(out var uiComponent))
                throw new Exception($"Failed to instantiate `{prefab.name}` UI prefab: the prefab doesn't contain a `{nameof(CustomUI)}` or `{nameof(IManagedUI)}` component on the root object.");

            uiComponent.SortingOrder += Configuration.SortingOffset;
            uiComponent.RenderMode = Configuration.RenderMode;
            uiComponent.RenderCamera = ObjectUtils.IsValid(customCamera) ? customCamera : ObjectUtils.IsValid(cameraManager.UICamera) ? cameraManager.UICamera : cameraManager.Camera;

            if (!string.IsNullOrEmpty(FontName) && Configuration.GetFontOption(FontName) is UIConfiguration.FontOption fontOption)
                uiComponent.SetFont(fontOption.Font, fontOption.TMPFont);
            if (FontSize > 0)
                uiComponent.SetFontSize(FontSize);

            var managedUI = new ManagedUI(name ?? prefab.name, gameObject, uiComponent);
            this.managedUI.Add(managedUI);

            await uiComponent.InitializeAsync();

            return uiComponent;
        }

        public virtual T GetUI<T> () where T : class, IManagedUI => GetUI(typeof(T)) as T;

        public virtual IManagedUI GetUI (Type type)
        {
            if (cachedGetUIResults.TryGetValue(type, out var cachedResult))
                return cachedResult;

            foreach (var managedUI in managedUI)
                if (type.IsAssignableFrom(managedUI.ComponentType))
                {
                    var result = managedUI.UIComponent;
                    cachedGetUIResults[type] = result;
                    return managedUI.UIComponent;
                }

            return null;
        }

        public virtual IManagedUI GetUI (string name)
        {
            foreach (var managedUI in managedUI)
                if (managedUI.Name == name)
                    return managedUI.UIComponent;
            return null;
        }

        public virtual bool RemoveUI (IManagedUI managedUI)
        {
            if (!this.managedUI.Any(u => u.UIComponent == managedUI))
                return false;

            var ui = this.managedUI.FirstOrDefault(u => u.UIComponent == managedUI);
            this.managedUI.Remove(ui);
            foreach (var kv in cachedGetUIResults.ToList())
            {
                if (kv.Value == managedUI)
                    cachedGetUIResults.Remove(kv.Key);
            }

            ObjectUtils.DestroyOrImmediate(ui.GameObject);

            return true;
        }

        public virtual void SetRenderMode (RenderMode renderMode, Camera renderCamera)
        {
            customCamera = renderCamera;
            foreach (var managedUI in managedUI)
            {
                managedUI.UIComponent.RenderMode = renderMode;
                managedUI.UIComponent.RenderCamera = renderCamera;
            }
        }

        public virtual void SetUIVisibleWithToggle (bool visible, bool allowToggle = true)
        {
            cameraManager.RenderUI = visible;

            var clickThroughPanel = GetUI<ClickThroughPanel>();
            if (clickThroughPanel)
            {
                if (visible) clickThroughPanel.Hide();
                else
                {
                    if (allowToggle) clickThroughPanel.Show(true, ToggleUI, InputConfiguration.SubmitName, InputConfiguration.ToggleUIName);
                    else clickThroughPanel.Show(false, null);
                }
            }
        }

        public virtual void SetModalUI (IManagedUI modalUI)
        {
            if (modalState.Count > 0) // Restore previous state.
            {
                foreach (var kv in modalState)
                    kv.Key.Interactable = kv.Value || (kv.Key is CustomUI customUI && customUI.ModalUI && customUI.Visible);
                modalState.Clear();
            }

            if (modalUI is null) return;

            foreach (var ui in managedUI)
            {
                modalState[ui.UIComponent] = ui.UIComponent.Interactable;
                ui.UIComponent.Interactable = false;
            }

            modalUI.Interactable = true;
        }

        protected virtual void SetFontName (string fontName)
        {
            if (FontName == fontName) return;

            this.fontName = fontName;

            if (string.IsNullOrEmpty(fontName))
            {
                foreach (var ui in managedUI)
                    ui.UIComponent.SetFont(null, null);
                return;
            }

            var fontOption = Configuration.GetFontOption(fontName);
            if (fontOption is null) throw new Exception($"Failed to set `{fontName}` font: Font option with the name is not assigned in the UI configuration.");

            foreach (var ui in managedUI)
                ui.UIComponent.SetFont(fontOption.Font, fontOption.TMPFont);
        }

        protected virtual void SetFontSize (int size)
        {
            if (fontSize == size) return;

            fontSize = size;

            foreach (var ui in managedUI)
                ui.UIComponent.SetFontSize(size);
        }

        protected virtual void ToggleUI () => SetUIVisibleWithToggle(!cameraManager.RenderUI);

        protected virtual async UniTask InstantiateUIsAsync ()
        {
            var resources = await loader.LoadAllAsync();
            var tasks = resources.Select(r => InstantiatePrefabAsync(r, loader.GetLocalPath(r)));
            await UniTask.WhenAll(tasks);
        }
    }
}
