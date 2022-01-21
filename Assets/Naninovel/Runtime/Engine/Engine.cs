﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

// Make sure none of the assembly types are stripped when building with IL2CPP.
[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]
[assembly: UnityEngine.Scripting.Preserve]

namespace Naninovel
{
    /// <summary>
    /// Class responsible for management of systems critical to the engine.
    /// </summary>
    public static class Engine
    {
        /// <summary>
        /// Invoked when the engine initialization is started.
        /// </summary>
        public static event Action OnInitializationStarted;
        /// <summary>
        /// Invoked when the engine initialization is finished.
        /// </summary>
        public static event Action OnInitializationFinished;

        /// <summary>
        /// Types (both built-in and user-created) available for the engine
        /// when looking for available actor implementations, serialization handlers, managed text, etc.
        /// </summary>
        /// <remarks>It's safe to access this property when the engine is not initialized.</remarks>
        public static IReadOnlyCollection<Type> Types => typesCache ?? (typesCache = GetEngineTypes());
        /// <summary>
        /// Configuration object used to initialize the engine.
        /// </summary>
        public static EngineConfiguration Configuration { get; private set; }
        /// <summary>
        /// Proxy <see cref="MonoBehaviour"/> used by the engine.
        /// </summary>
        public static IEngineBehaviour Behaviour { get; private set; }
        /// <summary>
        /// Composition root, containing all the engine-related game objects.
        /// </summary>
        public static GameObject RootObject => Behaviour.GetRootObject();
        /// <summary>
        /// Whether the engine is initialized and ready.
        /// </summary>
        public static bool Initialized => initializeTCS != null && initializeTCS.Task.IsCompleted();
        /// <summary>
        /// Whether the engine is currently being initialized.
        /// </summary>
        public static bool Initializing => initializeTCS != null && !initializeTCS.Task.IsCompleted();

        private static readonly List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        private static readonly List<IEngineService> services = new List<IEngineService>();
        private static readonly Dictionary<Type, IEngineService> cachedGetServiceResults = new Dictionary<Type, IEngineService>();
        private static readonly List<Func<UniTask>> preInitializationTasks = new List<Func<UniTask>>();
        private static readonly List<Func<UniTask>> postInitializationTasks = new List<Func<UniTask>>();
        private static IConfigurationProvider configurationProvider;
        private static UniTaskCompletionSource initializeTCS;
        private static IReadOnlyCollection<Type> typesCache;

        /// <summary>
        /// Adds an async function delegate to invoke before the engine initialization.
        /// Added delegates will be invoked and awaited in order before starting the initialization.
        /// </summary>
        public static void AddPreInitializationTask (Func<UniTask> task) => preInitializationTasks.Insert(0, task);

        /// <summary>
        /// Removes a delegate added via <see cref="AddPreInitializationTask(Func{UniTask})"/>.
        /// </summary>
        public static void RemovePreInitializationTask (Func<UniTask> task) => preInitializationTasks.Remove(task);

        /// <summary>
        /// Adds an async function delegate to invoke after the engine initialization.
        /// Added delegates will be invoked and awaited in order before finishing the initialization.
        /// </summary>
        public static void AddPostInitializationTask (Func<UniTask> task) => postInitializationTasks.Insert(0, task);

        /// <summary>
        /// Removes a delegate added via <see cref="AddPostInitializationTask(Func{UniTask})"/>.
        /// </summary>
        public static void RemovePostInitializationTask (Func<UniTask> task) => postInitializationTasks.Remove(task);

        /// <summary>
        /// Initializes engine behaviour and services.
        /// Services will be initialized in the order in which they were added to the list.
        /// </summary>
        /// <param name="configurationProvider">Configuration provider to use when resolving configuration objects.</param>
        /// <param name="behaviour">Unity's <see cref="MonoBehaviour"/> proxy to use.</param>
        /// <param name="services">List of engine services to initialize (order will be preserved).</param>
        public static async UniTask InitializeAsync (IConfigurationProvider configurationProvider, IEngineBehaviour behaviour, IList<IEngineService> services)
        {
            if (Initialized) return;
            if (Initializing) { await initializeTCS.Task; return; }

            initializeTCS = new UniTaskCompletionSource();
            OnInitializationStarted?.Invoke();

            for (int i = preInitializationTasks.Count - 1; i >= 0; i--)
            {
                await preInitializationTasks[i]();
                if (!Initializing) return; // In case initialization process was terminated (eg, exited playmode).
            }

            Engine.configurationProvider = configurationProvider;
            Configuration = GetConfiguration<EngineConfiguration>();

            Behaviour = behaviour;
            Behaviour.OnBehaviourDestroy += Destroy;

            objects.Clear();
            Engine.services.Clear();
            Engine.services.AddRange(services);

            for (var i = 0; i < Engine.services.Count; i++)
            {
                await Engine.services[i].InitializeServiceAsync();
                if (!Initializing) return;
            }

            for (int i = postInitializationTasks.Count - 1; i >= 0; i--)
            {
                await postInitializationTasks[i]();
                if (!Initializing) return;
            }

            initializeTCS?.TrySetResult();
            OnInitializationFinished?.Invoke();
        }

        /// <summary>
        /// Resets state of all the engine services.
        /// </summary>
        public static void Reset () => services.ForEach(s => s.ResetService());

        /// <summary>
        /// Resets state of engine services.
        /// </summary>
        /// <param name="exclude">Type of the engine services (interfaces) to exclude from reset.</param>
        public static void Reset (params Type[] exclude)
        {
            if (services.Count == 0) return;

            foreach (var service in services)
                if (exclude is null || exclude.Length == 0 || !exclude.Any(t => t.IsInstanceOfType(service)))
                    service.ResetService();
        }

        /// <summary>
        /// Deconstructs all the engine services and stops the behaviour.
        /// </summary>
        public static void Destroy ()
        {
            initializeTCS = null;

            services.ForEach(s => s.DestroyService());
            services.Clear();
            cachedGetServiceResults.Clear();

            if (Behaviour != null)
            {
                Behaviour.OnBehaviourDestroy -= Destroy;
                Behaviour.Destroy();
                Behaviour = null;
            }

            foreach (var obj in objects)
            {
                if (!obj) continue;
                var go = obj is GameObject gameObject ? gameObject : (obj as Component)?.gameObject;
                ObjectUtils.DestroyOrImmediate(go);
            }
            objects.Clear();

            Configuration = null;
            configurationProvider = null;
        }

        /// <summary>
        /// Attempts to provide a <see cref="Naninovel.Configuration"/> object of the specified type 
        /// via <see cref="IConfigurationProvider"/> used to initialize the engine.
        /// </summary>
        /// <typeparam name="T">Type of the requested configuration object.</typeparam>
        public static T GetConfiguration<T> () where T : Configuration => GetConfiguration(typeof(T)) as T;

        /// <summary>
        /// Attempts to provide a <see cref="Naninovel.Configuration"/> object of the provided type 
        /// via <see cref="IConfigurationProvider"/> used to initialize the engine.
        /// </summary>
        /// <param name="type">Type of the requested configuration object.</param>
        public static Configuration GetConfiguration (Type type)
        {
            if (configurationProvider is null)
                throw new Exception($"Failed to provide `{type.Name}` configuration object: Configuration provider is not available or the engine is not initialized.");

            return configurationProvider.GetConfiguration(type);
        }

        /// <summary>
        /// Attempts to resolve first matching <see cref="IEngineService"/> object from the services list using provided <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        /// Results per requested types are cached, so it's fine to use this method frequently without a <paramref name="predicate"/>.
        /// </remarks>
        /// <typeparam name="TService">Type of the requested service.</typeparam>
        /// <param name="predicate">Additional filter to apply when looking for a match.</param>
        /// <returns>First matching service or null, when no matches found.</returns>
        public static TService GetService<TService> (Predicate<TService> predicate = null)
            where TService : class, IEngineService
        {
            var requestedType = typeof(TService);

            if (predicate is null && cachedGetServiceResults.TryGetValue(requestedType, out var cachedResult))
                return cachedResult as TService;

            foreach (var service in services)
            {
                if (!requestedType.IsInstanceOfType(service)) continue;
                if (predicate != null && !predicate(service as TService)) continue;

                var result = service as TService;
                if (predicate is null)
                    cachedGetServiceResults[requestedType] = result;
                return result;
            }

            return null;
        }
        
        /// <inheritdoc cref="GetService{TService}(System.Predicate{TService})"/>
        /// <param name="result">First matching service or null, when no matches found.</param>
        /// <returns>whether a match was found.</returns>
        public static bool TryGetService<TService> (out TService result, Predicate<TService> predicate = null)
            where TService : class, IEngineService
        {
            result = GetService<TService>(predicate);
            return result != null;
        }

        /// <summary>
        /// Resolves all the matching <see cref="IEngineService"/> objects from the services list; returns empty list when no matches found.
        /// </summary>
        /// <typeparam name="TService">Type of the requested services.</typeparam>
        /// <param name="predicate">Additional filter to apply when looking for a match.</param>
        public static List<TService> GetAllServices<TService> (Predicate<TService> predicate = null) 
            where TService : class, IEngineService
        {
            var result = new List<TService>();
            var resolvingType = typeof(TService);

            var servicesOfType = services.FindAll(s => resolvingType.IsInstanceOfType(s));
            if (servicesOfType.Count > 0)
                result = servicesOfType.FindAll(s => predicate is null || predicate(s as TService)).Cast<TService>().ToList();

            return result;
        }

        /// <summary>
        /// Invokes <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/> and adds the object as child of the engine object.
        /// </summary>
        /// <param name="prototype">Prototype of the object to instantiate.</param>
        /// <param name="name">Name to assign for the instantiated object. Will use name of the prototype when not provided.</param>
        /// <param name="layer">Layer to assign for the instantiated object. When not provided and override layer is enabled in the engine configuration, will assign the layer specified in the configuration.</param>
        public static T Instantiate<T> (T prototype, string name = default, int? layer = default) where T : UnityEngine.Object
        {
            if (Behaviour is null)
                throw new Exception($"Failed to instantiate `{name ?? prototype.name}`: engine is not ready. " +
                                    $"Make sure you're not attempting to instantiate and object inside an engine service constructor (use `{nameof(IEngineService.InitializeServiceAsync)}` method instead).");

            var newObj = UnityEngine.Object.Instantiate(prototype);
            var gameObj = newObj is GameObject newGObj ? newGObj : (newObj as Component)?.gameObject;
            Behaviour.AddChildObject(gameObj);

            if (!string.IsNullOrEmpty(name)) newObj.name = name;

            if (layer.HasValue) gameObj.ForEachDescendant(obj => obj.layer = layer.Value);
            else if (Configuration.OverrideObjectsLayer) gameObj.ForEachDescendant(obj => obj.layer = Configuration.ObjectsLayer);

            objects.Add(newObj);

            return newObj;
        }

        /// <summary>
        /// Creates a new <see cref="GameObject"/>, making it a child of the engine object and (optionally) adding provided components.
        /// </summary>
        /// <param name="name">Name to assign for the instantiated object. Will use a default name when not provided.</param>
        /// <param name="layer">Layer to assign for the instantiated object. When not provided and override layer is enabled in the engine configuration, will assign the layer specified in the configuration.</param>
        /// <param name="components">Components to add on the created object.</param>
        public static GameObject CreateObject (string name = default, int? layer = default, params Type[] components)
        {
            if (Behaviour is null)
                throw new Exception($"Failed to create `{name ?? string.Empty}` object: engine is not ready. " +
                                    $"Make sure you're not attempting to create and object inside an engine service constructor (use `{nameof(IEngineService.InitializeServiceAsync)}` method instead).");

            var objName = name ?? "NaninovelObject";
            GameObject newObj;
            if (components != null) newObj = new GameObject(objName, components);
            else newObj = new GameObject(objName);
            Behaviour.AddChildObject(newObj);

            if (layer.HasValue) newObj.ForEachDescendant(obj => obj.layer = layer.Value);
            else if (Configuration.OverrideObjectsLayer) newObj.ForEachDescendant(obj => obj.layer = Configuration.ObjectsLayer);

            objects.Add(newObj);

            return newObj;
        }

        /// <summary>
        /// Creates a new <see cref="GameObject"/>, making it a child of the engine object and adding specified component type.
        /// </summary>
        /// <param name="name">Name to assign for the instantiated object. Will use a default name when not provided.</param>
        /// <param name="layer">Layer to assign for the instantiated object. When not provided and override layer is enabled in the engine configuration, will assign the layer specified in the configuration.</param>
        public static T CreateObject<T> (string name = default, int? layer = default) where T : Component
        {
            if (Behaviour is null)
                throw new Exception($"Failed to create `{name ?? string.Empty}` object of type `{typeof(T).Name}`: engine is not ready. " +
                                    $"Make sure you're not attempting to create and object inside an engine service constructor (use `{nameof(IEngineService.InitializeServiceAsync)}` method instead).");

            var newObj = new GameObject(name ?? typeof(T).Name);
            Behaviour.AddChildObject(newObj);

            if (layer.HasValue) newObj.ForEachDescendant(obj => obj.layer = layer.Value);
            else if (Configuration.OverrideObjectsLayer) newObj.ForEachDescendant(obj => obj.layer = Configuration.ObjectsLayer);

            objects.Add(newObj);

            return newObj.AddComponent<T>();
        }

        private static IReadOnlyCollection<Type> GetEngineTypes ()
        {
            var engineTypes = new List<Type>(1000);
            var engineConfig = ProjectConfigurationProvider.LoadOrDefault<EngineConfiguration>();
            var domainAssemblies = ReflectionUtils.GetDomainAssemblies(true, true, true);
            foreach (var assemblyName in engineConfig.TypeAssemblies)
            {
                var assembly = domainAssemblies.FirstOrDefault(a => a.FullName.StartsWithFast($"{assemblyName},"));
                if (assembly is null) continue;
                engineTypes.AddRange(assembly.GetExportedTypes());
            }
            return engineTypes;
        }
    }
}
