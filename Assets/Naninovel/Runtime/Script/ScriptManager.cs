﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <inheritdoc cref="IScriptManager"/>
    [InitializeAtRuntime]
    public class ScriptManager : IScriptManager
    {
        public event Action OnScriptLoadStarted;
        public event Action OnScriptLoadCompleted;

        public virtual ScriptsConfiguration Configuration { get; }
        public virtual string StartGameScriptName { get; private set; }
        public virtual bool CommunityModdingEnabled => Configuration.EnableCommunityModding;
        public virtual UI.ScriptNavigatorPanel ScriptNavigator { get; private set; }
        public virtual int TotalCommandsCount { get; private set; }

        private const string navigatorPrefabResourcesPath = "Naninovel/ScriptNavigator";

        private readonly IResourceProviderManager providerManager;
        private readonly ILocalizationManager localizationManager;
        private readonly Dictionary<string, Script> localizationScripts;
        private ResourceLoader<Script> scriptLoader, localeScriptLoader, externalScriptLoader;

        public ScriptManager (ScriptsConfiguration config, IResourceProviderManager providerManager, ILocalizationManager localizationManager)
        {
            Configuration = config;
            this.providerManager = providerManager;
            this.localizationManager = localizationManager;
            localizationScripts = new Dictionary<string, Script>();
        }

        public virtual async UniTask InitializeServiceAsync ()
        {
            scriptLoader = Configuration.Loader.CreateFor<Script>(providerManager);
            localeScriptLoader = Configuration.Loader.CreateLocalizableFor<Script>(providerManager, localizationManager, false);
            if (CommunityModdingEnabled)
                externalScriptLoader = Configuration.ExternalLoader.CreateFor<Script>(providerManager);

            if (Application.isPlaying && Configuration.EnableNavigator)
            {
                var navigatorPrefab = Resources.Load<UI.ScriptNavigatorPanel>(navigatorPrefabResourcesPath);
                ScriptNavigator = Engine.Instantiate(navigatorPrefab, "ScriptNavigator");
                ScriptNavigator.SortingOrder = Configuration.NavigatorSortOrder;
                ScriptNavigator.SetVisibility(false);
            }

            if (string.IsNullOrEmpty(Configuration.StartGameScript))
            {
                var scriptPaths = await scriptLoader.LocateAsync(string.Empty);
                StartGameScriptName = scriptPaths.FirstOrDefault();
            }
            else StartGameScriptName = Configuration.StartGameScript;

            if (Configuration.CountTotalCommands)
                TotalCommandsCount = await CountTotalCommandsAsync();
        }

        public virtual void ResetService () { }

        public virtual void DestroyService ()
        {
            if (ScriptNavigator)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(ScriptNavigator.gameObject);
                else UnityEngine.Object.DestroyImmediate(ScriptNavigator.gameObject);
            }
        }

        public virtual async UniTask<IReadOnlyCollection<Script>> LoadExternalScriptsAsync ()
        {
            if (!CommunityModdingEnabled)
                return new List<Script>();

            OnScriptLoadStarted?.Invoke();
            var scriptResources = await externalScriptLoader.LoadAllAsync();
            OnScriptLoadCompleted?.Invoke();
            return scriptResources.Select(r => r.Object).ToArray();
        }

        public virtual async UniTask<Script> LoadScriptAsync (string name)
        {
            OnScriptLoadStarted?.Invoke();

            if (scriptLoader.IsLoaded(name))
            {
                OnScriptLoadCompleted?.Invoke();
                return scriptLoader.GetLoadedOrNull(name);
            }

            var scriptResource = await scriptLoader.LoadAsync(name);
            if (!scriptResource.Valid) throw new Exception($"Failed to load `{name}` script: The resource is not available.");

            await TryAddLocalizationScriptAsync(scriptResource);

            OnScriptLoadCompleted?.Invoke();
            return scriptResource;
        }

        public virtual async UniTask<IReadOnlyCollection<Script>> LoadAllScriptsAsync ()
        {
            OnScriptLoadStarted?.Invoke();
            var scriptResources = await scriptLoader.LoadAllAsync();
            var scripts = scriptResources.Select(r => r.Object).ToArray();

            await UniTask.WhenAll(scripts.Select(TryAddLocalizationScriptAsync));

            if (ScriptNavigator)
                ScriptNavigator.GenerateScriptButtons(scripts);

            OnScriptLoadCompleted?.Invoke();
            return scripts;
        }

        public virtual void UnloadScript (string name)
        {
            if (scriptLoader.IsLoaded(name))
                scriptLoader.Unload(name);
            if (localizationScripts.ContainsKey(name))
            {
                localeScriptLoader?.Unload(name);
                localizationScripts.Remove(name);
            }
        }

        public virtual void UnloadAllScripts ()
        {
            scriptLoader.UnloadAll();
            foreach (var scriptName in localizationScripts.Keys)
                localeScriptLoader?.Unload(scriptName);
            localizationScripts.Clear();

            #if UNITY_GOOGLE_DRIVE_AVAILABLE
            // Delete cached scripts when using Google Drive resource provider.
            if (providerManager.ProviderInitialized(ResourceProviderConfiguration.GoogleDriveTypeName))
                (providerManager.GetProvider(ResourceProviderConfiguration.GoogleDriveTypeName) as GoogleDriveResourceProvider)?.PurgeCachedResources(Configuration.Loader.PathPrefix);
            #endif

            if (ScriptNavigator) ScriptNavigator.DestroyScriptButtons();
        }

        public virtual Script GetLocalizationScriptFor (Script script)
        {
            if (localizationManager.IsSourceLocaleSelected() || !localizationScripts.ContainsKey(script.Name)) return null;
            return localizationScripts[script.Name];
        }

        private async UniTask TryAddLocalizationScriptAsync (Script script)
        {
            if (script is null) return;
            
            if (await localeScriptLoader.ExistsAsync(script.Name))
            {
                var localizationScript = await localeScriptLoader.LoadAsync(script.Name);
                localizationScripts[script.Name] = localizationScript;
            }
        }

        private async UniTask<int> CountTotalCommandsAsync ()
        {
            var result = 0;

            var scripts = await LoadAllScriptsAsync();
            foreach (var script in scripts)
            {
                var playlist = new ScriptPlaylist(script.Name, script.ExtractCommands());
                result += playlist.Count;
            }

            return result;
        }
    }
}
