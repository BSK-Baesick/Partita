// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel
{
    /// <summary>
    /// Provides extension methods for <see cref="IScriptManager"/>.
    /// </summary>
    public static class ScriptManagerExtensions
    {
        /// <summary>
        /// Performs <see cref="IScriptManager.UnloadAllScripts"/> followed by <see cref="IScriptManager.LoadAllScriptsAsync"/>.
        /// </summary>
        public static async UniTask ReloadAllScriptsAsync (this IScriptManager manager)
        {
            manager.UnloadAllScripts();
            await manager.LoadAllScriptsAsync();
        }
    }
}
