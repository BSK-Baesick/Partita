using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("novel")]
public class SwitchToNovel: Command
{
    [RequiredParameter] public StringParameter difficulty;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        if(difficulty == "hard")
        {
            var duetCamera = GameObject.Find("duetHard/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = false;
        }

        if(difficulty == "normal")
        {
            var duetCamera = GameObject.Find("duetNormal/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = false;
        }

        if(difficulty == "easy")
        {
            var duetCamera = GameObject.Find("duetEasy/DuetCamera").GetComponent<Camera>();
            duetCamera.enabled = false;
        }

        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = true;

        if(difficulty == "hard")
        {
            var duetManager = GameObject.Find("duetHard");
            duetManager.SetActive(false);
        }

        if(difficulty == "normal")
        {
            var duetManager = GameObject.Find("duetNormal");
            duetManager.SetActive(false);
        }

        if(difficulty == "easy")
        {
            var duetManager = GameObject.Find("duetEasy");
            duetManager.SetActive(false);
        }

        var uiManager = Engine.GetService<IUIManager>();
        uiManager.SetUIVisibleWithToggle(true, true);

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
    }
}
