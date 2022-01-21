// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Represents a named state of an actor.
    /// </summary>
    [System.Serializable]
    public class ActorPose<TState>
        where TState : ActorState
    {
        /// <summary>
        /// Name (identifier) of the pose.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// Actor state associated with the pose.
        /// </summary>
        public TState ActorState => actorState;

        [SerializeField] private string name = default;
        [SerializeField] private TState actorState = default;
    }
}
