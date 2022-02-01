using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playBonfireSoundscape")]
public class PlayBonfireSoundscape : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartBonfireSoundscape();

        await UniTask.CompletedTask;
    }
}
