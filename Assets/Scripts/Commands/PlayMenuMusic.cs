using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

[CommandAlias("playMenuMusic")]
public class PlayMenuMusic : Command
{

    public override UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();

        audioManager.StartMenuMusic();

        return UniTask.CompletedTask;
    }
}
