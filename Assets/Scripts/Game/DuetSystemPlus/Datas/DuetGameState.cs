namespace DuetSystem.Datas
{
    /// <summary>
    /// Stores the state data for the duet system
    /// </summary>
    public enum DuetGameState
    {
        /// <summary>
        /// The game state for the short tutorial before the duet game runs
        /// </summary>
        ONBOARDING,

        /// <summary>
        /// The game state when the duet game is running
        /// </summary>
        RUNNING,

        /// <summary>
        /// The game state when the duet game ends
        /// </summary>
        TERMINATING
    }
}
