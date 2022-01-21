﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    [EditInProjectSettings]
    public class EngineConfiguration : Configuration
    {
        public const string DefaultGeneratedDataPath = "NaninovelData";
        public static readonly string[] DefaultTypeAssemblies = { "Assembly-CSharp", "Assembly-CSharp-Editor", "Elringus.Naninovel.Runtime", "Elringus.Naninovel.Editor" };

        [Tooltip("Relative (to the application data directory) path to store the automatically generated assets.")]
        public string GeneratedDataPath = DefaultGeneratedDataPath;
        [Tooltip("Whether to assign a specific layer to all the engine objects. Engine's camera will use the layer for the culling mask. Use this to isolate Naninovel objects from being rendered by other cameras.")]
        public bool OverrideObjectsLayer = false;
        [Tooltip("When `Override Objects Layer` is enabled, the specified layer will be assigned to all the engine objects.")]
        public int ObjectsLayer = 0;
        [Tooltip("Log type to use for UniTask-related exceptions.")]
        public LogType AsyncExceptionLogType = LogType.Error;
        [Tooltip("When looking for various types (eg, actor implementations, serialization handlers, managed text, etc) the engine will only scan the exported types of the specified assemblies for better performance. In case you're keeping your Naninovel-related types outside of Unity's predefined assemblies (using assembly definitions), add the assembly names here.\n\nWarning: Recompile the solution or restart Unity editor after modifying the list in order for changes to take effect.")]
        public string[] TypeAssemblies = DefaultTypeAssemblies;

        [Header("Initialization")]
        [Tooltip("Whether to automatically initialize the engine when application starts.")]
        public bool InitializeOnApplicationLoad = true;
        [Tooltip("Whether to apply `DontDestroyOnLoad` to the engine objects, making their lifetime independent of any loaded scenes. When disabled, the objects will be part of the Unity scene where the engine was initialized and will be destroyed when the scene is unloaded.")]
        public bool SceneIndependent = true;
        [Tooltip("Whether to show a loading UI while the engine is initializing.")]
        public bool ShowInitializationUI = true;
        [Tooltip("UI to show while the engine is initializing (when enabled). Will use a default one when not provided.")]
        public ScriptableUIBehaviour CustomInitializationUI = default;
        [Tooltip("Whether to automatically show title screen UI (main menu) after engine initialization. You can modify the title UI using UI customization feature (see online guide for more info).")]
        public bool ShowTitleUI = true;

        [Header("Development Console")]
        [Tooltip("Whether to enable development console.")]
        public bool EnableDevelopmentConsole = true;
        [Tooltip("Key used to toggle development console. You can also toggle it with a multi (3 or more) touch when using touchscreens.")]
        public KeyCode ToggleConsoleKey = KeyCode.BackQuote;
    }
}
