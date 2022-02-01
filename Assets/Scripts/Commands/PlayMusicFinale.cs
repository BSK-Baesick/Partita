using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMusicFinale")]
public class PlayMusicFinale : Command
{
    [RequiredParameter]
    private IntegerParameter milliaId;

    [RequiredParameter]
    private IntegerParameter stephanId;

    [RequiredParameter]
    private IntegerParameter veraId;

    [RequiredParameter]
    private IntegerParameter paschaId;

    [RequiredParameter]
    private IntegerParameter zurabId;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMusicFinale(milliaId, stephanId, veraId, paschaId, zurabId);

        await UniTask.CompletedTask;
    }
}
