// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    public class AudioSettings : ResourcefulSettings<AudioConfiguration>
    {
        protected override string HelpUri => "guide/audio.html#background-music";
        protected override Type ResourcesTypeConstraint => typeof(AudioClip);
        protected override string ResourcesCategoryId => Configuration.AudioLoader.PathPrefix;
        protected override string ResourcesSelectionTooltip => "Use `@bgm %name%` or `@sfx %name%` in naninovel scripts to play a background music or sound effect of the selected audio clip.";

        protected override void DrawConfigurationEditor ()
        {
            base.DrawConfigurationEditor();

            if (Configuration.EnableAutoVoicing && Configuration.AutoVoiceMode == AutoVoiceMode.ContentHash &&
                GUILayout.Button("Open Voice Map Utility", GUIStyles.NavigationButton)) VoiceMapWindow.OpenWindow();
        }

        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers ()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(AudioConfiguration.AutoVoiceMode)] = p => { if (Configuration.EnableAutoVoicing) EditorGUILayout.PropertyField(p); };
            return drawers;
        }

        [MenuItem("Naninovel/Resources/Audio")]
        private static void OpenResourcesWindow () => OpenResourcesWindowImpl();
    }
}
