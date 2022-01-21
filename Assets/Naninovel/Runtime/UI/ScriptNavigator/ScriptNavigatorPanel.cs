// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace Naninovel.UI
{
    public class ScriptNavigatorPanel : CustomUI
    {
        protected Transform ButtonsContainer => buttonsContainer;
        protected GameObject PlayButtonPrototype => playButtonPrototype;

        [SerializeField] private Transform buttonsContainer = null;
        [SerializeField] private GameObject playButtonPrototype = null;

        protected IScriptPlayer Player { get; private set; }
        protected IScriptManager ScriptManager { get; private set; }
        protected bool LoadedScriptsOnce { get; private set; }

        public override async UniTask ChangeVisibilityAsync (bool visible, float? duration = null, CancellationToken cancellationToken = default)
        {
            await base.ChangeVisibilityAsync(visible, duration, cancellationToken);
            if (cancellationToken.CancelASAP) return;
            
            if (visible && !LoadedScriptsOnce)
            {
                LoadedScriptsOnce = true;
                await LoadScriptsAsync();
            }
        }

        public virtual void GenerateScriptButtons (IEnumerable<Script> scripts)
        {
            DestroyScriptButtons();

            foreach (var script in scripts)
            {
                var scriptButton = Instantiate(playButtonPrototype, buttonsContainer, false);
                scriptButton.GetComponent<NavigatorPlayButton>().Initialize(this, script, Player);
            }
        }

        public virtual void DestroyScriptButtons () => ObjectUtils.DestroyAllChildren(buttonsContainer);

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(buttonsContainer, playButtonPrototype);
            Player = Engine.GetService<IScriptPlayer>();
            ScriptManager = Engine.GetService<IScriptManager>();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            Player.OnPlay += HandlePlay;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            Player.OnPlay -= HandlePlay;
        }

        protected virtual async UniTask LoadScriptsAsync () => await ScriptManager.LoadAllScriptsAsync();

        private void HandlePlay (Script script)
        {
            if (ScriptManager.Configuration.TitleScript == script.Name) return;
            Hide();
        } 
    } 
}
