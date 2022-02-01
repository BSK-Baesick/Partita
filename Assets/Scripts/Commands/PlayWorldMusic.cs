using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playWorldMusic")]
public class PlayWorldMusic : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartWorldMusic();

        await UniTask.CompletedTask;
    }
}
