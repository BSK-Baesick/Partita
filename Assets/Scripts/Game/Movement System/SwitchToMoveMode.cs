using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("moveMode")]
public class SwitchToMoveMode : Command
{
    public override async UniTask ExecuteAsync (CancellationToken asyncToken = default)
    {
        // 1. Disable Naninovel input.
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;

        // 2. Stop script player.
        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.Stop();

        // 3. Reset state.
        var stateManager = Engine.GetService<IStateManager>();
        await stateManager.ResetStateAsync();

        // 4. Switch cameras.
        var mCamera = GameObject.Find("Movement Camera").GetComponent<Camera>();
        mCamera.enabled = true;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = false;

        // 5. Enable character control.
        var controller = Object.FindObjectOfType<CharacterMovement>();
        controller.enabled = true;
        var col = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        col.enabled = true;
    }
}
