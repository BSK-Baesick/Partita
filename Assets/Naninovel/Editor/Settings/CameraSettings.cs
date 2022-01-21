// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Naninovel
{
    public class CameraSettings : ConfigurationSettings<CameraConfiguration>
    {
        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers ()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(CameraConfiguration.DefaultOrthoSize)] = p => { if (!Configuration.AutoCorrectOrthoSize) EditorGUILayout.PropertyField(p); };
            drawers[nameof(CameraConfiguration.Orthographic)] = p => { if (!ObjectUtils.IsValid(Configuration.CustomCameraPrefab)) EditorGUILayout.PropertyField(p); };
            drawers[nameof(CameraConfiguration.CustomUICameraPrefab)] = p => { if (Configuration.UseUICamera) EditorGUILayout.PropertyField(p); };
            return drawers;
        }
    }
}
