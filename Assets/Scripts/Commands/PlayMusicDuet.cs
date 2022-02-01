using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMusicDuet")]
public class PlayMusicDuet : Command
{
    [RequiredParameter]
    private IntegerParameter characterId;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMusicDuet(characterId);

        await UniTask.CompletedTask;
    }
}