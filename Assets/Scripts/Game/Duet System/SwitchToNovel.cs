using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("novel")]
public class SwitchToNovel: Command
{
    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var duetCamera = GameObject.Find("duet/DuetCamera").GetComponent<Camera>();
        duetCamera.enabled = false;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = true;

        var duetManager = GameObject.Find("duet");
        duetManager.SetActive(false);

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(true, true);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
    }
}
