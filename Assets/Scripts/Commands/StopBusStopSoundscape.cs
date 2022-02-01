using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("stopBusStopSoundscape")]
public class StopBusStopSoundscape : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.EndBusStopSoundscape();

        await UniTask.CompletedTask;
    }
}