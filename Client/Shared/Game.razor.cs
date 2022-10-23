namespace Blockchain.Client.Shared;

partial class Game : ComponentBase
{
    bool IsPlaying { get; set; } = false;

    Board Board { get; set; } = new();
}
