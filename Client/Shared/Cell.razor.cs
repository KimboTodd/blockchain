namespace Blockchain.Client.Shared;

partial class Cell : ComponentBase
{
    [Parameter]
    public Cell? CellParameter { get; set; }

    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Cell()
    {
    }

    public int Number { get; set; } = 0;
    public string DisplayNumber => Number.ToString();
    public int Row { get; set; }
    public int Column { get; set; }
    public string CSS { get; set; } = "";

    protected override void OnInitialized()
    {
        Console.WriteLine($"{CellParameter.Number} cell parameter");
        if (CellParameter is not null)
        {
            Number = CellParameter.Number;
        }
    }
}
