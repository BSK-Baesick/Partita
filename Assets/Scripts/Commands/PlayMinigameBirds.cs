using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameBirds")]
public class PlayMinigameBirds : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigameBirds();

        await UniTask.CompletedTask;
    }
}
