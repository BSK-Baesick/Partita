using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMinigameMillia")]
public class PlayMinigameMillia : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMinigameMillia();

        await UniTask.CompletedTask;
    }
}
