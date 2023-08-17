namespace Blockchain.Shared;

/// <summary>
/// Represents the current state of the game for things like showing game over 
/// or preventing moves when not appropriate.
/// </summary>
public enum GameState
{
    /// <summary>
    /// The game has not yet started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The game is currently in progress.
    /// </summary>
    Started,

    /// <summary>
    /// The game has ended.
    /// </summary>
    GameOver,
}