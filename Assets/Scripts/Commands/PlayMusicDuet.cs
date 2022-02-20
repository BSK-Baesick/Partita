using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playMusicDuet")]
public class PlayMusicDuet : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMusicDuet();

        await UniTask.CompletedTask;
    }
}