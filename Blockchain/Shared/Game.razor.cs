namespace Blockchain.Shared;

partial class Game : ComponentBase
{
    private bool IsPlaying { get; set; } = false;

    private bool IsGameOver { get; set; } = false;

    private Link? CurrentBlock { get; set; }

    public Link? NextLink { get; private set; }

    public int Score { get; private set; }

    public bool droppingAnimation { get; set; }

    private void Start()
    {
        Score = 0;
        IsPlaying = true;
        IsGameOver = false;
        NextLink = GenerateLink();

        GameLoop();
    }

    private void GameLoop()
    {
        // if we have dropped 7 links,
        // add a new row to the bottom,
        // if this would collide, game over
        // HandleBlockedRow

        CurrentBlock = NextLink;
        NextLink = GenerateLink();
    }

    private static Link GenerateLink()
    {
        var number = new Random(DateTime.Now.Millisecond).Next(1, 8);
        return new Link(7, 4) { Number = number };
    }

    private async Task KeyDown(KeyboardEventArgs e)
    {
        if (IsPlaying is false || CurrentBlock is null)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowRight":
                CurrentBlock.Column = CurrentBlock.Column >= 6
                    ? 6
                    : CurrentBlock.Column += 1;
                StateHasChanged();
                break;
            case "ArrowLeft":
                CurrentBlock.Column = CurrentBlock.Column <= 0
                    ? 0
                    : CurrentBlock.Column -= 1;
                StateHasChanged();
                break;
            case "ArrowDown":
            case " ":
                // CurrentBlock.Row -= 1;
                // Cells[1, 2] = CurrentBlock;
                // StateHasChanged();
                await HandleDropAsync();
                break;
            // case "m":
            //     await ToggleAudio();
            //     break;
        }
    }

    private async Task HandleDropAsync()
    {
        if (IsPlaying is false || CurrentBlock is null)
        {
            return;
        }

        droppingAnimation = true;

        var additionalScore = await DropInAsync(CurrentBlock);
        // consider changing this to an event to score as we go, might be more exciting
        Score += additionalScore;

        droppingAnimation = true;

        GameLoop();
    }

    private const int Size = 7;

    private Link?[,] Cells { get; set; } = new Link[Size, Size];

    protected override void OnInitialized()
    {
        // for (var i = 0; i < Size; i++)
        // for (var j = 0; j < Size; j++)
        //     Cells[i, j] = new Link(i, j);
    }

    private async Task<int> DropInAsync(Link current)
    {
        if (IsPlaying is false || CurrentBlock is null)
        {
            return 0;
        }

        if (Cells[6, CurrentBlock.Column] is not null)
        {
            IsGameOver = true;
            return 0;
        }

        while (CurrentBlock.Row > 0)
        {
            if (Cells[CurrentBlock.Row - 1, CurrentBlock.Column] is null || Cells[CurrentBlock.Row - 1, CurrentBlock.Column]?.DisplayNumber is null)
            {
                if (CurrentBlock.Row != 7)
                {
                    Cells[CurrentBlock.Row, CurrentBlock.Column] = null;
                }

                CurrentBlock.Row -= 1;

                Cells[CurrentBlock.Row, CurrentBlock.Column] = CurrentBlock;
            }

            StateHasChanged();
            await Task.Delay(50);
        }

        var score = scoreLinks();
        while (score > 0)
        {
            // delay to give animation time to play
            await Task.Delay(50);
            // remove all links that scored in cells

            // drop all links by 1 row until no more links can drop

            score = scoreLinks();
        }

        return score;
    }

    private int scoreLinks()
    {
        var linksBroken = 0;
        // for all consecutive chains in a column linksBroken++
        // mark this cell as scored
        // for all consecutive chains in a row linksBroken++
        // mark this cell as scored
        return linksBroken;
    }
}