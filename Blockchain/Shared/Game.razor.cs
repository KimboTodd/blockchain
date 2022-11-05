namespace Blockchain.Shared;

partial class Game : ComponentBase
{
    bool IsPlaying { get; set; } = false;

    // TODO: this should come from board
    bool IsGameOver { get; set; } = false;
    public Cell? CurrentBlock { get; private set; }
    public Cell? NextBlock { get; private set; }
    Board Board { get; set; } = new();
    public int Score { get; private set; }

    void Start()
    {
        Score = 0;

        IsPlaying = true;

        CurrentBlock = GenerateCell();
        NextBlock = GenerateCell();
    }

    private static Cell GenerateCell()
    {
        var rand = new Random(DateTime.Now.Millisecond);
        var number = rand.Next(1, 8);
        return new Cell(7, 4) { Number = number };
    }

    protected async Task KeyDown(KeyboardEventArgs e)
    {
        if (IsPlaying is false || CurrentBlock is null) { return; }

        switch (e.Key)
        {
            case "ArrowRight":
                CurrentBlock.Column = CurrentBlock.Column >= 7
                ? 7
                : CurrentBlock.Column += 1;
                break;
            case "ArrowLeft":
                CurrentBlock.Column = CurrentBlock.Column <= 1
                ? 1
                : CurrentBlock.Column -= 1;
                break;
            case "ArrowDown":
            case " ":
                await HandleDropAsync();
                break;
            // case "m":
            //     await ToggleAudio();
            //     break;
            default:
                break;
        }
    }

    private async Task HandleDropAsync()
    {
        if (IsPlaying is false || CurrentBlock is null) { return; }

        var additionalScore = await Board.DropInAsync(CurrentBlock);
        Score += additionalScore;

        CurrentBlock = GenerateCell();
        NextBlock = GenerateCell();
    }
}
