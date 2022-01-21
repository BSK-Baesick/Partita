﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Linq;
using UniRx.Async;

namespace Naninovel.Commands
{
    /// <summary>
    /// Resets state of the [engine services](https://naninovel.com/guide/engine-services.html) and unloads (disposes) 
    /// all the resources loaded by Naninovel (textures, audio, video, etc); will basically revert to an empty initial engine state.
    /// </summary>
    /// <remarks>
    /// The process is asynchronous and is masked with a loading screen ([ILoadingUI](https://naninovel.com/guide/user-interface.html#ui-customization)).
    /// <br/><br/>
    /// Be aware, that this command can not be undone (rewound back).
    /// </remarks>
    /// <example>
    /// ; Reset all the services.
    /// @resetState
    /// 
    /// ; Reset all the services except custom variable and audio managers,
    /// ; allowing currently played audio tracks continue playing.
    /// @resetState ICustomVariableManager,IAudioManager
    /// 
    /// ; Reset only the `ICharacterManager` and `IBackgroundManager` services.
    /// @resetState only:ICharacterManager,IBackgroundManager
    /// </example>
    public class ResetState : Command, Command.IForceWait
    {
        /// <summary>
        /// Names of the [engine services](https://naninovel.com/guide/engine-services.html) (interfaces) to exclude from reset.
        /// Consider adding `ICustomVariableManager` to preserve the local variables.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias)]
        public StringListParameter Exclude;
        /// <summary>
        /// Names of the [engine services](https://naninovel.com/guide/engine-services.html) (interfaces) to reset;
        /// other services won't be affected. Doesn't have effect when the nameless (exclude) parameter is assigned.
        /// </summary>
        public StringListParameter Only;

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            if (Assigned(Exclude)) await Engine.GetService<IStateManager>().ResetStateAsync(Exclude);
            else if (Assigned(Only))
            {
                var serviceTypes = Engine.GetAllServices<IEngineService>().Select(s => s.GetType()).ToArray();
                var onlyTypeNames = Only.Value.Select(v => v.Value);
                var onlyTypes = serviceTypes.Where(t => onlyTypeNames.Any(ot => ot == t.Name || t.GetInterface(ot) != null));
                var excludeTypes = serviceTypes.Where(t => !onlyTypes.Any(ot => ot.IsAssignableFrom(t))).ToArray();
                await Engine.GetService<IStateManager>().ResetStateAsync(excludeTypes);
            }
            else await Engine.GetService<IStateManager>().ResetStateAsync();
        }
    }
}
