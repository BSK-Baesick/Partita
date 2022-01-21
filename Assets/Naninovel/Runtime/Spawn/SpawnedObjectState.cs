// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naninovel
{
    [Serializable]
    public struct SpawnedObjectState : IEquatable<SpawnedObjectState>
    {
        public string Path => path;
        public string[] Parameters => parameters;

        [SerializeField] private string path;
        [SerializeField] private string[] parameters;

        public SpawnedObjectState (string path, string[] parameters)
        {
            this.path = path;
            this.parameters = parameters;
        }

        public override bool Equals (object obj)
        {
            return obj is SpawnedObjectState state && Equals(state);
        }

        public bool Equals (SpawnedObjectState other)
        {
            return Path == other.Path &&
                   EqualityComparer<string[]>.Default.Equals(Parameters, other.Parameters);
        }

        public override int GetHashCode ()
        {
            var hashCode = 289869881;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(Parameters);
            return hashCode;
        }

        public static bool operator == (SpawnedObjectState left, SpawnedObjectState right)
        {
            return left.Equals(right);
        }

        public static bool operator != (SpawnedObjectState left, SpawnedObjectState right)
        {
            return !(left == right);
        }
    }
}
