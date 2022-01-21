﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Naninovel
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Cached domain exported types from the non-dynamic assemblies.
        /// </summary>
        public static IReadOnlyCollection<Type> ExportedDomainTypes => cachedDomainTypes ?? (cachedDomainTypes = GetExportedDomainTypes());
        /// <summary>
        /// Cached domain exported types from the non-dynamic assemblies, excluding system and Unity assemblies.
        /// </summary>
        public static IReadOnlyCollection<Type> ExportedCustomTypes => cachedCustomTypes ?? (cachedCustomTypes = GetExportedDomainTypes(true, true, true));

        private static IReadOnlyCollection<Type> cachedDomainTypes;
        private static IReadOnlyCollection<Type> cachedCustomTypes;

        public static IReadOnlyCollection<Assembly> GetDomainAssemblies (bool excludeDynamic = true, bool excludeSystem = false, bool excludeUnity = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => 
                (!excludeDynamic || !a.IsDynamic) && 
                (!excludeSystem || !a.GlobalAssemblyCache && !a.FullName.StartsWithFast("System") && !a.FullName.StartsWithFast("mscorlib") && !a.FullName.StartsWithFast("netstandard")) &&
                (!excludeUnity || !a.FullName.StartsWithFast("UnityEditor") && !a.FullName.StartsWithFast("UnityEngine") && !a.FullName.StartsWithFast("Unity.") &&
                    !a.FullName.StartsWithFast("nunit.") && !a.FullName.StartsWithFast("ExCSS.") && !a.FullName.StartsWithFast("UniTask.") && 
                    !a.FullName.StartsWithFast("UniRx.") && !a.FullName.StartsWithFast("JetBrains.") && !a.FullName.StartsWithFast("Newtonsoft."))
            ).ToArray();
        }

        public static IReadOnlyCollection<Type> GetExportedDomainTypes (bool excludeDynamic = true, bool excludeSystem = false, bool excludeUnity = false)
        {
            return GetDomainAssemblies(excludeDynamic, excludeSystem, excludeUnity)
                .SelectMany(a => a.GetExportedTypes()).ToArray();
        }

        /// <summary>
        /// Uses <see cref="Type.GetField(string, BindingFlags)"/>, but also includes private fields from all the base types.
        /// In case multiple fields with equal names exist in different base types, will return only the first most-derived one.
        /// </summary>
        public static FieldInfo GetFieldWithInheritance (this Type type, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (type is null) return null;
            var field = type.GetField(fieldName, flags);
            return field ?? GetFieldWithInheritance(type.BaseType, fieldName, flags);
        }
    }
}
