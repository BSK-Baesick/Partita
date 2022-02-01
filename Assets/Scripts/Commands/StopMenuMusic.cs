using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("stopMenuMusic")]
public class StopMenuMusic : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.EndMenuMusic();

        await UniTask.CompletedTask;
    }
}
