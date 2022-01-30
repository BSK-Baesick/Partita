using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("PlayFModEvent")]
public class PlayFmodEvent : Command
{
    [RequiredParameter]
    private string fmodEventName;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventName);
    }
}
