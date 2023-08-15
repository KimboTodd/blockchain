﻿namespace Blockchain.Shared;

partial class Game : ComponentBase
{
    public int? NextNumber { get; private set; }

    public int Score { get; private set; }

    public bool droppingAnimation { get; set; }

    private bool IsPlaying { get; set; } = false;

    private bool IsGameOver { get; set; } = false;

    private Link? CurrentLink { get; set; }

    private const int IMPOSSIBLE_NUMBER = 8;

    private void Start()
    {
        Score = 0;
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

        // TODO: figure out what's going on with the link not displaying
        var thing = new Link(7, 4, NextNumber ?? IMPOSSIBLE_NUMBER);
        Console.WriteLine($"Current link: {thing.DisplayNumber}");
        CurrentLink = thing;
        var next = GenerateNumber();
        Console.WriteLine($"Next: {next}");
        NextNumber = next;
        StateHasChanged();
    }

    private static int GenerateNumber()
    {
        return new Random(DateTime.Now.Millisecond).Next(1, 2);

        // return new Random(DateTime.Now.Millisecond).Next(1, 8);
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

        var additionalScore = await DropInAsync(CurrentLink);
        // consider changing this to an event to score as we go, might be more exciting
        Score += additionalScore;

        droppingAnimation = false;

        GameLoop();
    }

    private const int Size = 7;

    private Link?[,] Cells { get; set; } = new Link[Size, Size];

    private async Task<int> DropInAsync(Link current)
    {
        if (IsPlaying is false || CurrentLink is null)
        {
            return 0;
        }

        if (Cells[6, CurrentLink.Column] is not null)
        {
            IsGameOver = true;
            return 0;
        }

        while (CurrentLink.Row > 0)
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

            StateHasChanged();
            await Task.Delay(50);
        }

        var score = scoreLinks();
        while (score > 0)
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

            // drop all links by 1 row until no more links can drop


            score = scoreLinks();
        }

        return score;
    }

    private int scoreLinks()
    {
        var linksBroken = 0;
        // Loop through rows left to right scanning horizontally for consecutive filled chains
        for (var i = 0; i < Cells.GetLength(0); i++)
        {
            int? chainStart = null;
            int? chainEnd = null;
            for (var j = 0; j < Cells.GetUpperBound(1); j++)
                // if we encounter a filled cell
            {
                if (Cells[i, j] is not null)
                {
                    // if chain start is null, set it to this cell index
                    if (chainStart is null)
                    {
                        chainStart = j;
                    }
                    // else if chain start is not null, increment the end of the chain marker
                    else if (chainEnd is not null)
                    {
                        chainEnd = j;
                    }
                }
                else
                {
                    // we have encountered an empty cell
                    // if chain start is not null, we have found the end of the chain
                    if (chainStart is not null)
                    {
                        chainEnd = j;
                        var consecutiveChainLength = chainEnd - chainStart;
                        // traverse this chain from chainStart to chainEnd
                        for (var k = chainStart.GetValueOrDefault(); k < chainEnd; k++)
                        {
                            // and mark any cels within that number == consecutiveChainLength as scored
                            if (Cells[i, k].Number == consecutiveChainLength)
                            {
                                Cells[i, k].Scored = true;
                                linksBroken++;
                            }
                        }

                        // reset chain start and end to null
                        chainStart = null;
                        chainEnd = null;
                    }
                }
            }

            // Loop through columns top to bottom scanning vertically for consecutive filled cells
        }

        return linksBroken;
    }
}