﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using Naninovel.UI;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IChoiceHandlerActor"/> implementation using <see cref="UI.ChoiceHandlerPanel"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(ChoiceHandlerPanel), false)]
    public class UIChoiceHandler : MonoBehaviourActor<ChoiceHandlerMetadata>, IChoiceHandlerActor
    {
        public override string Appearance { get; set; }
        public override bool Visible { get => HandlerPanel.Visible; set => HandlerPanel.Visible = value; }
        public virtual List<ChoiceState> Choices { get; } = new List<ChoiceState>();

        protected virtual ChoiceHandlerPanel HandlerPanel { get; private set; }

        private readonly IStateManager stateManager;
        private readonly IUIManager uiManager;

        public UIChoiceHandler (string id, ChoiceHandlerMetadata metadata)
            : base(id, metadata)
        {
            stateManager = Engine.GetService<IStateManager>();
            uiManager = Engine.GetService<IUIManager>();
        }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();

            var providerManager = Engine.GetService<IResourceProviderManager>();
            var prefabResource = await ActorMetadata.Loader.CreateFor<GameObject>(providerManager).LoadAsync(Id);
            if (!prefabResource.Valid) throw new Exception($"Failed to load `{Id}` choice handler resource object. Make sure the handler is correctly configured.");

            var uiManager = Engine.GetService<IUIManager>();
            HandlerPanel = await uiManager.InstantiatePrefabAsync(prefabResource.Object) as ChoiceHandlerPanel;
            if (HandlerPanel == null) throw new Exception($"Failed to initialize `{Id}` choice handler actor: choice panel UI instantiation failed.");
            HandlerPanel.OnChoice += HandleChoice;
            HandlerPanel.transform.SetParent(Transform);

            Visible = false;
        }

        public override UniTask ChangeAppearanceAsync (string appearance, float duration, EasingType easingType = default,
            Transition? transition = default, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public override async UniTask ChangeVisibilityAsync (bool visible, float duration, EasingType easingType = default, CancellationToken cancellationToken = default)
        {
            if (HandlerPanel)
                await HandlerPanel.ChangeVisibilityAsync(visible, duration);
        }

        public virtual void AddChoice (ChoiceState choice)
        {
            Choices.Add(choice);
            HandlerPanel.AddChoiceButton(choice);
        }

        public virtual void RemoveChoice (string id)
        {
            Choices.RemoveAll(c => c.Id == id);
            HandlerPanel.RemoveChoiceButton(id);
        }

        public virtual ChoiceState GetChoice (string id) => Choices.FirstOrDefault(c => c.Id == id);

        public override void Dispose ()
        {
            base.Dispose();

            if (HandlerPanel != null)
            {
                uiManager.RemoveUI(HandlerPanel);
                ObjectUtils.DestroyOrImmediate(HandlerPanel.gameObject);
                HandlerPanel = null;
            }
        }

        protected override Color GetBehaviourTintColor () => Color.white;

        protected override void SetBehaviourTintColor (Color tintColor) { }

        protected virtual async void HandleChoice (ChoiceState state)
        {
            if (!Choices.Exists(c => c.Id.EqualsFast(state.Id))) return;

            stateManager.PeekRollbackStack()?.AllowPlayerRollback();

            Choices.Clear();

            if (HandlerPanel)
            {
                HandlerPanel.RemoveAllChoiceButtonsDelayed(); // Delayed to allow custom onClick logic.
                HandlerPanel.Hide();
            }

            var player = Engine.GetService<IScriptPlayer>();
            var script = Script.FromScriptText($"`{Id}` on choice script", state.OnSelectScript);
            var playlist = new ScriptPlaylist(script);
            await player.PlayTransientAsync(playlist);
                
            if (state.AutoPlay && !player.Playing)
            {
                var nextIndex = player.PlayedIndex + 1;
                player.Play(player.Playlist, nextIndex);
                return;
            }
        }
    } 
}
