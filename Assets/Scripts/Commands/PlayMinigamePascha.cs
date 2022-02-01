using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigamePascha")]
public class PlayMinigamePascha : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigamePascha();

        await UniTask.CompletedTask;
    }
}
