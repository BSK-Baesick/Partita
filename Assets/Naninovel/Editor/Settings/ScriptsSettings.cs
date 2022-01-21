// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Naninovel
{
    public class ScriptsSettings : ResourcefulSettings<ScriptsConfiguration>
    {
        protected override string HelpUri => "guide/naninovel-scripts.html";

        protected override Type ResourcesTypeConstraint => typeof(Script);
        protected override string ResourcesCategoryId => Configuration.Loader.PathPrefix;
        protected override bool AllowRename => false;
        protected override string ResourcesSelectionTooltip => "Use `@goto %name%` in naninovel scripts to load and start playing selected naninovel script.";

        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers ()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(ScriptsConfiguration.InitializationScript)] = p => EditorResources.DrawPathPopup(p, ResourcesCategoryId, ResourcesPathPrefix, "None (disabled)");
            drawers[nameof(ScriptsConfiguration.TitleScript)] = p => EditorResources.DrawPathPopup(p, ResourcesCategoryId, ResourcesPathPrefix, "None (disabled)");
            drawers[nameof(ScriptsConfiguration.StartGameScript)] = p => EditorResources.DrawPathPopup(p, ResourcesCategoryId, ResourcesPathPrefix);
            drawers[nameof(ScriptsConfiguration.ExternalLoader)] = p => { if (Configuration.EnableCommunityModding) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.ShowNavigatorOnInit)] = p => { if (Configuration.EnableNavigator) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.NavigatorSortOrder)] = p => { if (Configuration.EnableNavigator) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.HideUnusedParameters)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.InsertLineKey)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.InsertLineModifier)] = property => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(property); };
            drawers[nameof(ScriptsConfiguration.SaveScriptKey)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.SaveScriptModifier)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.EditorPageLength)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            drawers[nameof(ScriptsConfiguration.EditorCustomStyleSheet)] = p => { if (Configuration.EnableVisualEditor) EditorGUILayout.PropertyField(p); };
            return drawers;
        }

        [MenuItem("Naninovel/Resources/Scripts")]
        private static void OpenResourcesWindow () => OpenResourcesWindowImpl();
    }
}
