// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    [CustomEditor(typeof(LayeredActorBehaviour), true)]
    public class LayeredActorBehaviourEditor : Editor
    {
        private const string mapFieldName = "compositionMap";
        private const string buildMethodName = "BuildLayerData";

        private void OnEnable ()
        {
            EditorApplication.contextualPropertyMenu += HandlePropertyContextMenu;
        }

        private void OnDisable ()
        {
            EditorApplication.contextualPropertyMenu -= HandlePropertyContextMenu;
        }

        private void HandlePropertyContextMenu (GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Generic || 
                !property.propertyPath.Contains($"{mapFieldName}.Array.data[")) return;

            var propertyCopy = property.Copy();
            menu.AddItem(new GUIContent("Preview Composition"), false, () =>
            {
                var targetObj = propertyCopy.serializedObject.targetObject as LayeredActorBehaviour;
                if (targetObj == null) return;
                var map = typeof(LayeredActorBehaviour).GetField(mapFieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(targetObj) as List<LayeredActorBehaviour.CompositionMapItem>;
                if (map == null) return;
                var index = propertyCopy.propertyPath.GetAfterFirst($"{mapFieldName}.Array.data[").GetBefore("]").AsInvariantInt();
                if (index != null)
                {
                    typeof(LayeredActorBehaviour).GetMethod(buildMethodName, BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(targetObj, null);
                    var mapItem = map[index.Value];
                    targetObj.ApplyComposition(mapItem.Composition);
                }

                EditorUtility.SetDirty(propertyCopy.serializedObject.targetObject);
            });
        }
    }
}
