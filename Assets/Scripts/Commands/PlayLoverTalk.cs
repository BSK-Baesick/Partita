using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playLoverTalk")]
public class PlayLoverTalk : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartLoverTalk();

        await UniTask.CompletedTask;
    }
}
