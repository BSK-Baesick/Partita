using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameStephan")]
public class PlayMinigameStephan : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigameStephan();

        await UniTask.CompletedTask;
    }
}
