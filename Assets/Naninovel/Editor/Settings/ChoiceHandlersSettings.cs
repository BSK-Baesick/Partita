﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEditor;

namespace Naninovel
{
    public class ChoiceHandlersSettings : ActorManagerSettings<ChoiceHandlersConfiguration, IChoiceHandlerActor, ChoiceHandlerMetadata>
    {
        protected override string HelpUri => "guide/choices.html";
        protected override string ResourcesSelectionTooltip => GetTooltip();

        private string GetTooltip ()
        {
            if (EditedActorId == Configuration.DefaultHandlerId)
                return "Use `@choice \"Choice summary text.\"` in naninovel scripts to add a choice with this handler.";
            return $"Use `@choice \"Choice summary text.\" handler:{EditedActorId}` in naninovel scripts to add a choice with this handler.";
        }

        [MenuItem("Naninovel/Resources/Choice Handlers")]
        private static void OpenResourcesWindow () => OpenResourcesWindowImpl();
    }
}
