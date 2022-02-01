using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameGettingReady")]
public class PlayMinigameGettingReady : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMiniGameGettingReady();

        await UniTask.CompletedTask;
    }
}
