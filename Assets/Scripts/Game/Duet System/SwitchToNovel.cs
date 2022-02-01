using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("novel")]
public class SwitchToNovel: Command
{
    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var duetManager = GameObject.Find("duet/DuetManager");
        duetManager.SetActive(false);

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(true, true);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
    }
}
