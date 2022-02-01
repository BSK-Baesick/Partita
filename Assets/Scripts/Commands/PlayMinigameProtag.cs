using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameProtag")]
public class PlayMinigameProtag : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigameProtag();

        await UniTask.CompletedTask;
    }
}
