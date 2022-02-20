using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

[CommandAlias("setDuetCharacter")]
public class SetDuetCharacter : Command
{

    [RequiredParameter]
    private StringParameter characterId;

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();

        variableManager.SetVariableValue("currentCharacterPlaying", characterId);

        await UniTask.CompletedTask;
    }
}
