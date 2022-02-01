using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("playClickUI")]
public class PlayClickUI : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartClickUI();

        await UniTask.CompletedTask;
    }
}