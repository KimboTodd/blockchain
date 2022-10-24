namespace Blockchain.Client.Shared;

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
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Cells[i, j] = new();
            }
        }
    }

    internal async Task<int> DropInAsync(Cell current)
    {
        if (current is null) { return 0; }

        while (current.Row > 1)
        {
            // TODO: should the CanDrop be a method called from the game and the 
            // board can remain ignorant of the game state?
            // Can if move down? 
            if (Cells[6, current.Column] is null)
            {
                Console.WriteLine("Game Over");
                IsGameOver = true;
                return 0;
            }

            current.Row -= 1;
            Cells[current.Row, current.Column] = current;

            await Task.Delay(20);
        }

        return Score();
    }

    private int Score()
    {
        return 1;
    }
}


