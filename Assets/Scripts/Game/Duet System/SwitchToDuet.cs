using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("duet")]
public class SwitchToDuet: Command
{
    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var duetCamera = GameObject.Find("duet/DuetCamera").GetComponent<Camera>();
        duetCamera.enabled = true;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = false;

        var duetManager = GameObject.Find("duet");
        duetManager.SetActive(true);

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(false, false);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;
    }
}