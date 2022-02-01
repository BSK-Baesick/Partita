using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("stopWorldMusic")]
public class StopWorldMusic : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.EndWorldMusic();

        await UniTask.CompletedTask;
    }
}
