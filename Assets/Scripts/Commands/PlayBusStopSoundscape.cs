using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playBusStopSoundscape")]
public class playBusStopSoundscape : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartBusStopSoundscape();

        await UniTask.CompletedTask;
    }
}
