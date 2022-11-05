namespace Blockchain.Shared;

partial class Board : ComponentBase
{
    private const int Size = 7;

    [Parameter]
    public bool IsGameOver { get; set; }

    [Parameter]
    public Cell? CurrentBlock { get; set; }

    Cell[,] Cells { get; set; } = new Cell[Size, Size];

    protected override void OnInitialized()
    {
        Console.WriteLine("IN on initialized");
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Cells[i, j] = new Cell(i, j);
            }
        }
    }

    internal async Task<int> DropInAsync(Cell current)
    {
        if (current is null) { return 0; }

        if (Cells[6, current.Column] is not null)
        {
            Console.WriteLine("Game Over");
            IsGameOver = true;
            return 0;
        }

        while (current.Row > 1)
        {
            // TODO: should the CanDrop be a method called from the game and the 
            // board can remain ignorant of the game state?

            var cellBelow = Cells[current.Row - 1, current.Column];
            if (cellBelow is null)
            {
                if (current.Row != 6)
                {
                    Cells[6, current.Column] = new Cell();
                }

                current.Row = current.Row - 1;
                Console.WriteLine($"{current.DisplayNumber} display number, {current.Row} current row");

                Cells[current.Row, current.Column].Number = current.Number;
                Console.WriteLine(Cells[current.Row, current.Column].DisplayNumber);
            }

            // StateHasChanged();
            await Task.Delay(40);

        }

        return Score();
    }

    private int Score()
    {
        return 1;
    }
}


