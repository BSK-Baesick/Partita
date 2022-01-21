// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Naninovel
{
    public abstract class OrthoActorManagerSettings<TConfig, TActor, TMeta> : ActorManagerSettings<TConfig, TActor, TMeta>
        where TConfig : OrthoActorManagerConfiguration<TMeta>
        where TActor : IActor
        where TMeta : OrthoActorMetadata
    {
        protected override Dictionary<string, Action<SerializedProperty>> OverrideMetaDrawers ()
        {
            var drawers = base.OverrideMetaDrawers();
            drawers[nameof(OrthoActorMetadata.Pivot)] = p => { if (ResourcesTypeConstraint != null) EditorGUILayout.PropertyField(p); };
            drawers[nameof(OrthoActorMetadata.PixelsPerUnit)] = p => { if (ResourcesTypeConstraint != null) EditorGUILayout.PropertyField(p); };
            drawers[nameof(OrthoActorMetadata.EnableDepthPass)] = p => { if (ResourcesTypeConstraint != null) EditorGUILayout.PropertyField(p); };
            drawers[nameof(OrthoActorMetadata.DepthAlphaCutoff)] = p => { if (ResourcesTypeConstraint != null && EditedMetadata.EnableDepthPass) EditorGUILayout.PropertyField(p); };
            drawers[nameof(OrthoActorMetadata.CustomShader)] = p => { if (ResourcesTypeConstraint != null && !typeof(GenericActorBehaviour).IsAssignableFrom(ResourcesTypeConstraint)) EditorGUILayout.PropertyField(p); };
            return drawers;
        }
    }
}
