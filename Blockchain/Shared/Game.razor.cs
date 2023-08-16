namespace Blockchain.Shared;

partial class Game : ComponentBase
{
    private ScoreKeeper scoreKeeper;

    private int? NextNumber { get; set; }

    private bool droppingAnimation { get; set; }

    private bool IsPlaying { get; set; } = false;

    private bool IsGameOver { get; set; } = false;

    private Link? CurrentLink { get; set; }

    private Link?[,] Cells { get; set; } = new Link[SIZE, SIZE];

    private const int IMPOSSIBLE_NUMBER = 8;

    private const int SIZE = 7;

    private void Start()
    {
        IsPlaying = true;
        IsGameOver = false;
        NextNumber = GenerateNumber();

        GameLoop();
    }

    private void GameLoop()
    {
        // if we have dropped 7 links,
        // add a new row to the bottom,
        // if this would collide, game over
        // HandleBlockedRow

        CurrentLink = new Link(7, 4, NextNumber ?? IMPOSSIBLE_NUMBER);
        NextNumber = GenerateNumber();
        StateHasChanged();
    }

    private static int GenerateNumber()
    {
        return new Random(DateTime.Now.Millisecond).Next(1, 8);
    }

    private async Task KeyDown(KeyboardEventArgs e)
    {
        if (IsPlaying is false || CurrentLink is null)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowRight":
                if (droppingAnimation)
                {
                    return;
                }

                CurrentLink.Column = CurrentLink.Column >= 6
                    ? 6
                    : CurrentLink.Column += 1;
                StateHasChanged();
                break;
            case "ArrowLeft":
                if (droppingAnimation)
                {
                    return;
                }

                CurrentLink.Column = CurrentLink.Column <= 0
                    ? 0
                    : CurrentLink.Column -= 1;
                StateHasChanged();
                break;
            case "ArrowDown":
            case " ":
                if (droppingAnimation)
                {
                    return;
                }

                await HandleDropAsync();
                break;
        }
    }

    private async Task HandleDropAsync()
    {
        if (IsPlaying is false || CurrentLink is null)
        {
            return;
        }

        droppingAnimation = true;

        await DropInAsync(CurrentLink);

        droppingAnimation = false;

        GameLoop();
    }

    private async Task DropInAsync(Link current)
    {
        if (IsPlaying is false || CurrentLink is null)
        {
            return;
        }

        if (Cells[6, CurrentLink.Column] is not null)
        {
            IsGameOver = true;
            return;
        }

        var collided = false;
        while (CurrentLink.Row > 0 && collided == false)
        {
            if (Cells[CurrentLink.Row - 1, CurrentLink.Column] is null || Cells[CurrentLink.Row - 1, CurrentLink.Column]?.DisplayNumber is null)
            {
                if (CurrentLink.Row != 7)
                {
                    Cells[CurrentLink.Row, CurrentLink.Column] = null;
                }

                CurrentLink.Row -= 1;

                Cells[CurrentLink.Row, CurrentLink.Column] = CurrentLink;
            }
            else
            {
                collided = true;
            }

            StateHasChanged();
            await Task.Delay(50);
        }

        var linksScoring = scoreLinks();
        while (linksScoring is true)
        {
            // delay to give animation time to play
            await Task.Delay(200);

            // remove all links that scored in cells
            for (var i = 0; i < Cells.GetLength(0); i++)
            {
                for (var j = 0; j < Cells.GetUpperBound(1); j++)
                {
                    if (Cells[i, j]?.Scored is true)
                    {
                        Cells[i, j] = null;
                    }
                }
            }

            // TODO: drop all links by 1 row until no more links can drop

            linksScoring = scoreLinks();
        }
    }

    private bool scoreLinks()
    {
        var linksBroken = 0;
        // Loop through rows left to right scanning horizontally for consecutive filled chains
        for (var i = 0; i < Cells.GetLength(0); i++)
        {
            int? chainStart = null;
            for (var j = 0; j < Cells.GetUpperBound(1); j++)
            {
                // if we encounter a filled cell
                if (Cells[i, j] is not null)
                {
                    // if chainStart is null, set it to this cell index
                    if (chainStart is null)
                    {
                        chainStart = j;
                    }
                }
                else
                {
                    // we have encountered an empty cell
                    // if chainStart is not null, we have found the end of the chain
                    if (chainStart is not null)
                    {
                        var chainEnd = j;
                        var consecutiveChainLength = chainEnd - chainStart;
                        // traverse this chain from chainStart to chainEnd
                        for (var k = chainStart.GetValueOrDefault(); k < chainEnd; k++)
                        {
                            // and mark any cells within that number == consecutiveChainLength as scored
                            if (Cells[i, k].Number == consecutiveChainLength)
                            {
                                Cells[i, k].Scored = true;
                                linksBroken++;
                            }
                        }

                        // reset chain start and end to null
                        chainStart = null;
                    }
                }
            }
        }

        // Loop through columns top to bottom scanning vertically for consecutive filled cells
        for (var j = 0; j < Cells.GetUpperBound(1); j++)
        {
            int? chainStart = null;
            for (var i = 0; i < Cells.GetLength(0); i++)
            {
                // if we encounter a filled cell
                if (Cells[i, j] is not null)
                {
                    // if chainStart is null, set it to this cell index
                    if (chainStart is null)
                    {
                        chainStart = i;
                    }
                }
                else
                {
                    // we have encountered an empty cell
                    // if chainStart is not null, we have found the end of the chain
                    if (chainStart is not null)
                    {
                        var chainEnd = i;
                        var consecutiveChainLength = chainEnd - chainStart;
                        // traverse this chain from chainStart to chainEnd
                        for (var k = chainStart.GetValueOrDefault(); k < chainEnd; k++)
                        {
                            // and mark any cels within that number == consecutiveChainLength as scored
                            if (Cells[k, j].Number == consecutiveChainLength)
                            {
                                Cells[k, j].Scored = true;
                                linksBroken++;
                            }
                        }

                        // reset chain start and end to null
                        chainStart = null;
                    }
                }
            }
        }

        scoreKeeper.OnLinksBroken(linksBroken);
        return linksBroken > 0;
    }
}