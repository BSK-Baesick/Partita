// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    [CustomPropertyDrawer(typeof(ResourcePopupAttribute))]
    public class ResourcesPopupPropertyDrawer : PropertyDrawer
    {
        private EditorResources editorResources;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            if (!editorResources)
                editorResources = EditorResources.LoadOrDefault();

            var attr = attribute as ResourcePopupAttribute;
            editorResources.DrawPathPopup(position, property, attr.Category, attr.PathPrefix, attr.EmptyOption);
        }
    }
}
