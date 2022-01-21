﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    public static class GUIContents
    {
        public static readonly GUIContent HelpIcon;

        static GUIContents ()
        {
            var contentsType = typeof(EditorGUI).GetNestedType("GUIContents", BindingFlags.NonPublic);

            HelpIcon = contentsType.GetProperty("helpIcon", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as GUIContent;
        }
    }
}
