using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("novel")]
public class SwitchToNovelMode : Command
{
    public StringParameter ScriptName;
    public StringParameter Label;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var controller = Object.FindObjectOfType<CharacterMovement>();
        controller.enabled = false;
        var col = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        col.enabled = false;

        var mCamera = GameObject.Find("Movement Camera").GetComponent<Camera>();
        mCamera.enabled = false;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = true;

        if (Assigned(ScriptName))
        {
            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            await scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label);
        }

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
        
    }
}
