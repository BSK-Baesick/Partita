// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <inheritdoc cref="ICameraManager"/>
    [InitializeAtRuntime(-1)] // Add camera at the start, so the user could see something while waiting for the engine init.
    public class CameraManager : ICameraManager, IStatefulService<GameStateMap>, IStatefulService<SettingsStateMap>
    {
        [Serializable]
        public class Settings
        {
            public int QualityLevel = -1;
        }

        [Serializable]
        public class GameState
        {
            [Serializable]
            public struct CameraComponent
            {
                public string TypeName;
                public bool Enabled;

                public CameraComponent (MonoBehaviour comp)
                {
                    TypeName = comp.GetType().Name;
                    Enabled = comp.enabled;
                }
            }

            public float OrthoSize = -1f;
            public Vector3 Offset = Vector3.zero;
            public Quaternion Rotation = Quaternion.identity;
            public float Zoom = 0f;
            public bool Orthographic = true;
            public CameraLookController.State LookMode = default;
            public CameraComponent[] CameraComponents;
            public bool RenderUI = true;
        }

        public event Action<float> OnAspectChanged;

        public virtual CameraConfiguration Configuration { get; }
        public virtual Vector2Int Resolution => new Vector2Int(Screen.width, Screen.height);
        public virtual Camera Camera { get; protected set; }
        public virtual Camera UICamera { get; protected set; }
        public virtual bool RenderUI
        {
            get => Configuration.UseUICamera ? UICamera.enabled : MaskUtils.GetLayer(Camera.cullingMask, uiLayer);
            set { if (Configuration.UseUICamera) UICamera.enabled = value; else Camera.cullingMask = MaskUtils.SetLayer(Camera.cullingMask, uiLayer, value); }
        }
        public virtual Vector3 Offset
        {
            get => offset;
            set { CompleteOffsetTween(); offset = value; ApplyOffset(value); }
        }
        public virtual Quaternion Rotation
        {
            get => rotation;
            set { CompleteRotationTween(); rotation = value; ApplyRotation(value); }
        }
        public virtual float Zoom
        {
            get => zoom;
            set { CompleteZoomTween(); zoom = value; ApplyZoom(value); }
        }
        public virtual bool Orthographic
        {
            get => Camera.orthographic;
            set { Camera.orthographic = value; Zoom = Zoom; }
        }
        public virtual float OrthoSize { get => orthoSize; set { ApplyOrthoSizeZoomAware(value, Zoom); orthoSize = value; } }
        public virtual int QualityLevel { get => QualitySettings.GetQualityLevel(); set => QualitySettings.SetQualityLevel(value, true); }

        protected virtual float AspectRatio => this.GetAspectRatio();

        private readonly IInputManager inputManager;
        private readonly IEngineBehaviour engineBehaviour;
        private readonly RenderTexture thumbnailRenderTexture;
        private readonly List<MonoBehaviour> cameraComponentsCache = new List<MonoBehaviour>();
        private readonly Tweener<VectorTween> offsetTweener = new Tweener<VectorTween>();
        private readonly Tweener<VectorTween> rotationTweener = new Tweener<VectorTween>();
        private readonly Tweener<FloatTween> zoomTweener = new Tweener<FloatTween>();
        private CameraLookController lookController;
        private float lastAspect;
        private float orthoSize;
        private Vector3 offset = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private float zoom = 0f;
        private int uiLayer;

        public CameraManager (CameraConfiguration config, IInputManager inputManager, IEngineBehaviour engineBehaviour)
        {
            Configuration = config;
            this.inputManager = inputManager;
            this.engineBehaviour = engineBehaviour;

            thumbnailRenderTexture = new RenderTexture(config.ThumbnailResolution.x, config.ThumbnailResolution.y, 24);
        }

        public virtual UniTask InitializeServiceAsync ()
        {
            // Do it here and not in ctor to allow camera initialize first.
            // Otherwise, when starting the game, for a moment, no cameras will be available for render.
            uiLayer = Engine.GetConfiguration<UIConfiguration>().ObjectsLayer;

            if (ObjectUtils.IsValid(Configuration.CustomCameraPrefab))
                Camera = Engine.Instantiate(Configuration.CustomCameraPrefab);
            else
            {
                Camera = Engine.CreateObject<Camera>(nameof(CameraManager));
                Camera.depth = 0;
                Camera.backgroundColor = new Color32(35, 31, 32, 255);
                Camera.orthographic = Configuration.Orthographic;
                if (!Configuration.UseUICamera)
                    Camera.allowHDR = false; // Otherwise text artifacts appear when printing.
                if (Engine.Configuration.OverrideObjectsLayer) // When culling is enabled, render only the engine object and UI (when not using UI camera) layers.
                    Camera.cullingMask = Configuration.UseUICamera ? 1 << Engine.Configuration.ObjectsLayer : (1 << Engine.Configuration.ObjectsLayer) | (1 << uiLayer);
                else if (Configuration.UseUICamera) Camera.cullingMask = ~(1 << uiLayer);
            }
            Camera.transform.position = Configuration.InitialPosition;

            if (Configuration.UseUICamera)
            {
                if (ObjectUtils.IsValid(Configuration.CustomUICameraPrefab))
                    UICamera = Engine.Instantiate(Configuration.CustomUICameraPrefab);
                else
                {
                    UICamera = Engine.CreateObject<Camera>("UICamera");
                    UICamera.depth = 1;
                    UICamera.orthographic = true;
                    UICamera.allowHDR = false; // Otherwise text artifacts appear when printing.
                    UICamera.cullingMask = 1 << uiLayer;
                    UICamera.clearFlags = CameraClearFlags.Depth;
                }
                UICamera.transform.position = Configuration.InitialPosition;
            }

            lastAspect = AspectRatio;
            if (Configuration.AutoCorrectOrthoSize)
                CorrectOrthoSize(lastAspect);
            else OrthoSize = Configuration.DefaultOrthoSize;

            lookController = new CameraLookController(this, inputManager.GetCameraLookX(), inputManager.GetCameraLookY());

            engineBehaviour.OnBehaviourLateUpdate += MonitorAspect;
            engineBehaviour.OnBehaviourUpdate += lookController.Update;

            return UniTask.CompletedTask;
        }

        public virtual void ResetService ()
        {
            lookController.Enabled = false;
            Offset = Vector3.zero;
            Rotation = Quaternion.identity;
            Zoom = 0f;
            Orthographic = Configuration.Orthographic;
        }

        public virtual void DestroyService ()
        {
            if (engineBehaviour != null)
            {
                engineBehaviour.OnBehaviourLateUpdate -= MonitorAspect;
                engineBehaviour.OnBehaviourUpdate -= lookController.Update;
            }

            ObjectUtils.DestroyOrImmediate(thumbnailRenderTexture);
            if (Camera != null) ObjectUtils.DestroyOrImmediate(Camera.gameObject);
            if (UICamera != null) ObjectUtils.DestroyOrImmediate(UICamera.gameObject);
        }

        public virtual void SaveServiceState (SettingsStateMap stateMap)
        {
            var settings = new Settings {
                QualityLevel = QualityLevel
            };
            stateMap.SetState(settings);
        }

        public virtual UniTask LoadServiceStateAsync (SettingsStateMap stateMap)
        {
            var settings = stateMap.GetState<Settings>() ?? new Settings();
            if (settings.QualityLevel >= 0 && settings.QualityLevel != QualityLevel)
                QualityLevel = settings.QualityLevel;

            return UniTask.CompletedTask;
        }

        public virtual void SaveServiceState (GameStateMap stateMap)
        {
            Camera.gameObject.GetComponents(cameraComponentsCache);
            var gameState = new GameState {
                OrthoSize = OrthoSize,
                Offset = Offset,
                Rotation = Rotation,
                Zoom = Zoom,
                Orthographic = Orthographic,
                LookMode = lookController.GetState(),
                RenderUI = RenderUI,
                // Why zero? Camera is not a MonoBehaviour, so don't count it; others are considered to be custom effect.
                CameraComponents = cameraComponentsCache.Count > 0 ? cameraComponentsCache.Select(c => new GameState.CameraComponent(c)).ToArray() : null
            };
            stateMap.SetState(gameState);
        }

        public virtual UniTask LoadServiceStateAsync (GameStateMap stateMap)
        {
            var state = stateMap.GetState<GameState>();
            if (state is null)
            {
                ResetService();
                return UniTask.CompletedTask;
            }
            
            if (state.OrthoSize > 0) OrthoSize = state.OrthoSize;
            Offset = state.Offset;
            Rotation = state.Rotation;
            Zoom = state.Zoom;
            Orthographic = state.Orthographic;
            RenderUI = state.RenderUI;
            SetLookMode(state.LookMode.Enabled, state.LookMode.Zone, state.LookMode.Speed, state.LookMode.Gravity);

            if (state.CameraComponents != null)
                foreach (var compState in state.CameraComponents)
                {
                    var comp = Camera.gameObject.GetComponent(compState.TypeName) as MonoBehaviour;
                    if (comp == null) continue;
                    comp.enabled = compState.Enabled;
                }

            return UniTask.CompletedTask;
        }

        public virtual void SetLookMode (bool enabled, Vector2 lookZone, Vector2 lookSpeed, bool gravity)
        {
            lookController.Enabled = enabled;
            lookController.LookZone = lookZone;
            lookController.LookSpeed = lookSpeed;
            lookController.Gravity = gravity;
        }

        public virtual Texture2D CaptureThumbnail ()
        {
            if (Configuration.HideUIInThumbnails)
                RenderUI = false;

            // Hide the save-load menu in case it's visible.
            var saveLoadUI = Engine.GetService<IUIManager>()?.GetUI<UI.ISaveLoadUI>();
            var saveLoadUIWasVisible = saveLoadUI?.Visible;
            if (saveLoadUIWasVisible.HasValue && saveLoadUIWasVisible.Value)
                saveLoadUI.Visible = false;

            // Confirmation UI may still be visible here (due to a fade-out time); force-hide it.
            var confirmUI = Engine.GetService<IUIManager>()?.GetUI<UI.IConfirmationUI>();
            var confirmUIWasVisible = confirmUI?.Visible ?? false;
            if (confirmUI != null) confirmUI.Visible = false;

            var initialRenderTexture = Camera.targetTexture;
            Camera.targetTexture = thumbnailRenderTexture;
            Camera.Render();
            Camera.targetTexture = initialRenderTexture;

            if (RenderUI && Configuration.UseUICamera)
            {
                initialRenderTexture = UICamera.targetTexture;
                UICamera.targetTexture = thumbnailRenderTexture;
                UICamera.Render();
                UICamera.targetTexture = initialRenderTexture;
            }

            var thumbnail = thumbnailRenderTexture.ToTexture2D();

            // Restore the save-load menu and confirmation UI in case we hid them.
            if (saveLoadUIWasVisible.HasValue && saveLoadUIWasVisible.Value)
                saveLoadUI.Visible = true;
            if (confirmUIWasVisible)
                confirmUI.Visible = true;

            if (Configuration.HideUIInThumbnails)
                RenderUI = true;

            return thumbnail;
        }

        public virtual async UniTask ChangeOffsetAsync (Vector3 offset, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            CompleteOffsetTween();

            if (duration > 0)
            {
                var currentOffset = this.offset;
                this.offset = offset;
                var tween = new VectorTween(currentOffset, offset, duration, ApplyOffset, false, easingType, Camera);
                await offsetTweener.RunAsync(tween, cancellationToken);
            }
            else Offset = offset;
        }

        public virtual async UniTask ChangeRotationAsync (Quaternion rotation, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            CompleteRotationTween();

            if (duration > 0)
            {
                var currentRotation = this.rotation;
                this.rotation = rotation;
                var tween = new VectorTween(currentRotation.ClampedEulerAngles(), rotation.ClampedEulerAngles(), duration, ApplyRotation, false, easingType, Camera);
                await rotationTweener.RunAsync(tween, cancellationToken);
            }
            else Rotation = rotation;
        }

        public virtual async UniTask ChangeZoomAsync (float zoom, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            CompleteZoomTween();

            if (duration > 0)
            {
                var currentZoom = this.zoom;
                this.zoom = zoom;
                var tween = new FloatTween(currentZoom, zoom, duration, ApplyZoom, false, easingType, Camera);
                await zoomTweener.RunAsync(tween, cancellationToken);
            }
            else Zoom = zoom;
        }

        private void MonitorAspect ()
        {
            if (!Mathf.Approximately(lastAspect, AspectRatio))
            {
                OnAspectChanged?.Invoke(AspectRatio);
                lastAspect = AspectRatio;
                if (Configuration.AutoCorrectOrthoSize)
                    CorrectOrthoSize(lastAspect);
            }
        }

        /// <summary>
        /// Changes current <see cref="OrthoSize"/> to accommodate the provided aspect ratio. 
        /// </summary>
        private void CorrectOrthoSize (float aspect)
        {
            OrthoSize = Mathf.Clamp(Configuration.ReferenceResolution.x / aspect / 200f, 0f, Configuration.MaxOrthoSize);
        }

        /// <summary>
        /// Sets the provided ortho size to the camera respecting the provided zoom level.
        /// </summary>
        private void ApplyOrthoSizeZoomAware (float size, float zoom)
        {
            Camera.orthographicSize = size * (1f - Mathf.Clamp(zoom, 0, .99f));
        }

        private void ApplyOffset (Vector3 offset)
        {
            Camera.transform.position = Configuration.InitialPosition + offset;
        }

        private void ApplyRotation (Quaternion rotation)
        {
            Camera.transform.rotation = rotation;
        }

        private void ApplyRotation (Vector3 rotation)
        {
            Camera.transform.rotation = Quaternion.Euler(rotation);
        }

        private void ApplyZoom (float zoom)
        {
            if (Orthographic) ApplyOrthoSizeZoomAware(OrthoSize, zoom);
            else Camera.fieldOfView = Mathf.Lerp(5f, 60f, 1f - zoom);
        }

        private void CompleteOffsetTween ()
        {
            if (offsetTweener.Running)
                offsetTweener.CompleteInstantly();
        }

        private void CompleteRotationTween ()
        {
            if (rotationTweener.Running)
                rotationTweener.CompleteInstantly();
        }

        private void CompleteZoomTween ()
        {
            if (zoomTweener.Running)
                zoomTweener.CompleteInstantly();
        }
    } 
}
