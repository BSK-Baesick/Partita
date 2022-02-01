using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameZurab")]
public class PlayMinigameZurab : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigameZurab();

        await UniTask.CompletedTask;
    }
}
