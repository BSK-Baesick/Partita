// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Base class for project-specific configuration assets used to initialize and configure services and other engine systems.
    /// Serialized configuration assets are generated automatically under the engine's data folder and can be edited via Unity project settings menu
    /// when the implementation has an <see cref="EditInProjectSettingsAttribute"/> applied.
    /// </summary>
    public abstract class Configuration : ScriptableObject
    {
        /// <summary>
        /// When applied to a <see cref="Configuration"/> implementation, adds an associated editor settings menu.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class EditInProjectSettingsAttribute : Attribute { }
    }
}
