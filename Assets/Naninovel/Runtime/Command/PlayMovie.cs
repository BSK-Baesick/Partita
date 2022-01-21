// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel.Commands
{
    /// <summary>
    /// Plays a movie with the provided name (path).
    /// </summary>
    /// <remarks>
    /// Will fade-out the screen before playing the movie and fade back in after the play.
    /// Playback can be canceled by activating a `cancel` input (`Esc` key by default).
    /// </remarks>
    /// <example>
    /// ; Given an "Opening" video clip is added to the movie resources, plays it
    /// @movie Opening
    /// </example>
    [CommandAlias("movie")]
    public class PlayMovie : Command, Command.IPreloadable
    {
        /// <summary>
        /// Name of the movie resource to play.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), RequiredParameter, IDEResource(MoviesConfiguration.DefaultMoviesPathPrefix)]
        public StringParameter MovieName;

        protected IMoviePlayer Player => Engine.GetService<IMoviePlayer>();

        public async UniTask PreloadResourcesAsync ()
        {
            if (!Assigned(MovieName) || MovieName.DynamicValue) return;
            await Player.HoldResourcesAsync(MovieName, this);
        }

        public void ReleasePreloadedResources ()
        {
            if (!Assigned(MovieName) || MovieName.DynamicValue) return;
            Player?.ReleaseResources(MovieName, this);
        }

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            await Player.PlayAsync(MovieName, cancellationToken);
        }
    }
}
