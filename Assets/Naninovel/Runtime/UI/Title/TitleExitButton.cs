// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;
using UnityEngine;

namespace Naninovel.UI
{
    public class TitleExitButton : ScriptableButton
    {
        private const string titleLabel = "OnExit";

        private string titleScriptName;
        private IScriptPlayer scriptPlayer;
        private IScriptManager scriptManager;

        protected override void Awake ()
        {
            base.Awake();

            scriptManager = Engine.GetService<IScriptManager>();
            titleScriptName = scriptManager.Configuration.TitleScript;
            scriptPlayer = Engine.GetService<IScriptPlayer>();
        }

        protected override async void OnButtonClick ()
        {
            if (!string.IsNullOrEmpty(titleScriptName) &&
                await scriptManager.LoadScriptAsync(titleScriptName) is Script titleScript &&
                titleScript.LabelExists(titleLabel))
            {
                await scriptPlayer.PreloadAndPlayAsync(titleScriptName, label: titleLabel);
                await UniTask.WaitWhile(() => scriptPlayer.Playing);
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer)
                Application.OpenURL("about:blank");
            else Application.Quit();
        }
    }
}
