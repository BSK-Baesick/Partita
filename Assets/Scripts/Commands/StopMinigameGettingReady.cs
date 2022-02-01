using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("stopMinigameGettingReady")]
public class StopMinigameGettingReady : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.EndMiniGameGettingReady();

        await UniTask.CompletedTask;
    }
}
