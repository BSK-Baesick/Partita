// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    public class StateSettings : ConfigurationSettings<StateConfiguration>
    {
        private static readonly string[] gameHandlerImplementations, gameHandlerImplementationsLabels;
        private static readonly string[] globalHandlerImplementations, globalHandlerImplementationsLabels;
        private static readonly string[] settingsHandlerImplementations, settingsHandlerImplementationsLabels;

        static StateSettings ()
        {
            InitializeHandlerOptions<ISaveSlotManager<GameStateMap>>(ref gameHandlerImplementations, ref gameHandlerImplementationsLabels);
            InitializeHandlerOptions<ISaveSlotManager<GlobalStateMap>>(ref globalHandlerImplementations, ref globalHandlerImplementationsLabels);
            InitializeHandlerOptions<ISaveSlotManager<SettingsStateMap>>(ref settingsHandlerImplementations, ref settingsHandlerImplementationsLabels);
        }
        
        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers ()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(StateConfiguration.StateRollbackSteps)] = p => { if (Configuration.EnableStateRollback) EditorGUILayout.PropertyField(p); };
            drawers[nameof(StateConfiguration.SavedRollbackSteps)] = p => { if (Configuration.EnableStateRollback) EditorGUILayout.PropertyField(p); };
            drawers[nameof(StateConfiguration.GameStateHandler)] = property =>
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Serialization Handlers", EditorStyles.boldLabel);
                DrawHandlersDropdown(property, gameHandlerImplementations, gameHandlerImplementationsLabels);
            };
            drawers[nameof(StateConfiguration.GlobalStateHandler)] = p => DrawHandlersDropdown(p, globalHandlerImplementations, globalHandlerImplementationsLabels);
            drawers[nameof(StateConfiguration.SettingsStateHandler)] = p => DrawHandlersDropdown(p, settingsHandlerImplementations, settingsHandlerImplementationsLabels);
            return drawers;
        }

        private static void DrawHandlersDropdown (SerializedProperty property, string[] values, string[] labels)
        {
            var label = EditorGUI.BeginProperty(Rect.zero, null, property);
            var curIndex = ArrayUtility.IndexOf(values, property.stringValue ?? string.Empty);
            var newIndex = EditorGUILayout.Popup(label, curIndex, labels);
            property.stringValue = values.IsIndexValid(newIndex) ? values[newIndex] : string.Empty;
            EditorGUI.EndProperty();
        }

        private static void InitializeHandlerOptions<THandler> (ref string[] values, ref string[] labels) where THandler : ISaveSlotManager
        {
            values = Engine.Types
                .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(THandler)))
                .Select(t => t.AssemblyQualifiedName).ToArray();
            labels = values.Select(s => s.GetBefore(",")).ToArray();
        }
    }
}
