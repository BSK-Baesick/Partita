using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("stopBonfireSoundscape")]
public class StopBonfireSoundscape : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.EndBonfireSoundscape();

        await UniTask.CompletedTask;
    }
}
