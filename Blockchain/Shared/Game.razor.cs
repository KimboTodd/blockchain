namespace Blockchain.Shared;

public partial class Game : ComponentBase
{
    private ElementReference gameBoardDiv;

    private ScoreKeeper? scoreKeeper;

    private const int IMPOSSIBLE_NUMBER = 9;

    private const int SIZE = 7;

    private const int BLOCKED_LINK = 8;

    private int TotalLinks { get; set; }

    private int? NextNumber { get; set; }

    private bool DroppingAnimation { get; set; }

    private GameState GameState { get; set; } = GameState.NotStarted;

    private Link? CurrentLink { get; set; }

    private Link?[,] Cells { get; set; } = new Link[SIZE, SIZE];

    private async Task Start()
    {
        GameState = GameState.Started;
        NextNumber = GenerateNumber();

        // Focus the browser on the grid board div
        await gameBoardDiv.FocusAsync();

        GameLoop();
    }

    private void GameLoop()
    {
        CurrentLink = new Link(7, 4, NextNumber ?? IMPOSSIBLE_NUMBER);
        TotalLinks += 1;

        // TODO: if we have dropped 7 links, add a new blocked row to the bottom
        if (TotalLinks % 7 == 0)
        {
            var shiftedCells = new Link[SIZE, SIZE];

            // Looping top to bottom - [row, column]
            for (var i = Cells.GetLength(0) - 1; i >= 0; i--)
            {
                for (var j = 0; j < Cells.GetUpperBound(1); j++)
                {
                    if (i == Cells.GetLength(0) && Cells[i, j] is not null)
                    {
                        // in the first row, if anything is not null, game over
                        GameState = GameState.GameOver;
                        break;
                    }

                    // Last row add new blocked row
                    if (i == 0)
                    {
                        shiftedCells[i, j] = new Link(i, j, BLOCKED_LINK);
                    }
                    else if (Cells[i - 1, j] is not null)
                    {
                        // shift all rows up one
                        shiftedCells[i, j] = Cells[i - 1, j];
                    }
                }
            }

            Cells = shiftedCells;
        }

        NextNumber = GenerateNumber();
        StateHasChanged();
    }

    private static int GenerateNumber()
    {
        return new Random(DateTime.Now.Millisecond).Next(1, 8);
    }

    private async Task KeyDown(KeyboardEventArgs e)
    {
        if (GameState != GameState.Started || CurrentLink is null)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowRight":
                if (DroppingAnimation)
                {
                    return;
                }

                CurrentLink.Column = CurrentLink.Column >= 6
                    ? 6
                    : CurrentLink.Column += 1;
                StateHasChanged();
                break;
            case "ArrowLeft":
                if (DroppingAnimation)
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
                if (DroppingAnimation)
                {
                    return;
                }

                await HandleDropAsync();
                break;
        }
    }

    private async Task HandleDropAsync()
    {
        if (GameState != GameState.Started || CurrentLink is null)
        {
            return;
        }

        DroppingAnimation = true;

        await DropCurrentLinkAsync(CurrentLink);

        // Handle scoring and possible combos
        var linksScoring = ScoreLinks();
        while (linksScoring is true)
        {
            // delay to give animation time to play
            await Task.Delay(150);

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

            var cellsShifted = await ShiftCellsDown();
            while (cellsShifted) cellsShifted = await ShiftCellsDown();

            linksScoring = ScoreLinks();
        }

        DroppingAnimation = false;

        GameLoop();
    }

    private async Task<bool> ShiftCellsDown()
    {
        var cellsShifted = false;
        for (var j = 0; j < Cells.GetUpperBound(1); j++)
        {
            // loop through each column starting at the bottom
            for (var i = 0; i < Cells.GetLength(0) - 1; i++)
            {
                // if we encounter a null cell and the one above it is not null
                if (Cells[i, j] is null && Cells[i + 1, j] is not null)
                {
                    cellsShifted = true;
                    Cells[i, j] = Cells[i + 1, j];
                    Cells[i + 1, j] = null;
                }
            }
        }

        StateHasChanged();
        await Task.Delay(150);
        return cellsShifted;
    }

    private async Task DropCurrentLinkAsync(Link current)
    {
        if (GameState != GameState.Started || CurrentLink is null)
        {
            return;
        }

        if (Cells[6, CurrentLink.Column] is not null)
        {
            GameState = GameState.GameOver;
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
            await Task.Delay(40);
        }
    }

    private bool ScoreLinks()
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
                            // and mark any cells within that number == consecutiveChainLength as scored
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

        scoreKeeper.OnLinksBrokenAsync(linksBroken);
        return linksBroken > 0;
    }
}