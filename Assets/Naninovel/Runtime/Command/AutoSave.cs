// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel.Commands
{
    /// <summary>
    /// Automatically save the game to a quick save slot.
    /// </summary>
    /// <example>
    /// @save
    /// </example>
    [CommandAlias("save")]
    public class AutoSave : Command
    {
        public override UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            // Don't await here, otherwise script player won't be able to sync the running commands.
            Engine.GetService<IStateManager>().QuickSaveAsync().Forget();
            return UniTask.CompletedTask;
        }
    } 
}
