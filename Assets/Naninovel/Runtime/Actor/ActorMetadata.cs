﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Represents serializable data required to construct and initialize a <see cref="IActor"/>.
    /// </summary>
    [System.Serializable]
    public abstract class ActorMetadata
    {
        /// <summary>
        /// Globally-unique identifier of the medata instance.
        /// </summary>
        public string Guid => guid;

        [Tooltip("Assembly-qualified type name of the actor implementation.")]
        public string Implementation = default;
        [Tooltip("Data describing how to load actor's resources.")]
        public ResourceLoaderConfiguration Loader = default;

        [HideInInspector]
        [SerializeField] private string guid = System.Guid.NewGuid().ToString();
        [SerializeReference] private CustomMetadata customData = default;

        /// <summary>
        /// Attempts to retrieve an actor state associated with the provided pose name;
        /// returns null when not found.
        /// </summary>
        public abstract TState GetPoseOrNull<TState> (string poseName) where TState : ActorState;
        /// <summary>
        /// Attempts to retrieve a custom data of type <typeparamref name="TData"/>.
        /// </summary>
        /// <typeparam name="TData">Type of the custom data to retrieve.</typeparam>
        public virtual TData GetCustomData<TData> () where TData : CustomMetadata
        {
            return customData as TData;
        }
    }
}
