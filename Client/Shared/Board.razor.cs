namespace Blockchain.Client.Shared;

partial class Board : ComponentBase
{
    private const int Size = 7;

    [Parameter]
    public bool IsPlaying { get; set; }

    Cell[,] ArrayCells { get; set; } = new Cell[Size, Size];

    void Start()
    {

        IsPlaying = true;
    }

    protected override void OnInitialized()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                ArrayCells[i, j] = new();
            }

        }
    }
}


