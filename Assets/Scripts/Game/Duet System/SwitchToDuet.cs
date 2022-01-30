using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("duet")]
public class SwitchToDuet: Command
{
    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var duetManager = GameObject.Find("duet/DuetManager");
        duetManager.SetActive(true);

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(false, false);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;
    }
}