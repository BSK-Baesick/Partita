using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("duet")]
public class SwitchToDuet: Command
{
    [RequiredParameter] public StringParameter difficulty;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        if(difficulty == "hard")
        {
            var duetCamera = GameObject.Find("duetHard/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = true;
        }

        if(difficulty == "normal")
        {
            var duetCamera = GameObject.Find("duetNormal/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = true;
        }

        if(difficulty == "easy")
        {
            var duetCamera = GameObject.Find("duetEasy/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = true;
        }

        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = false;

        if(difficulty == "hard")
        {
            var duetManager = GameObject.Find("duetHard");
            duetManager.SetActive(true);
        }

        if(difficulty == "normal")
        {
            var duetManager = GameObject.Find("duetNormal");
            duetManager.SetActive(true);
        }

        if(difficulty == "easy")
        {
            var duetManager = GameObject.Find("duetEasy");
            duetManager.SetActive(true);
        }

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(false, false);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;
    }
}