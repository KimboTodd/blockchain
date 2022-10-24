namespace Blockchain.Client.Shared;

partial class Board : ComponentBase
{
    private const int Size = 7;

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

    internal async Task<int> DropInAsync(Cell currentBlock)
    {
        if (currentBlock is null)
        {
            return 0;
        }

        while (currentBlock.Row > 1)
        {
            currentBlock.Row -= 1;
            await Task.Delay(20);
        }

        return Score();
    }

    private int Score()
    {
        return 1;
    }
}


