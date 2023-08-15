using System.Diagnostics;

namespace Blockchain.Shared;

[DebuggerDisplay("Row: {Row}, Column: {Column} Number: {Number}")]
partial class Link : ComponentBase
{
    [Parameter]
    public Link? CellParameter { get; set; }

    [Parameter]
    public int? Number { get; set; }

    public Link(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Link()
    {
    }

    // private int _number;

    // {
    //     get => _number; set
    //     {
    //         if (_number != value)
    //         {
    //             _number = value;
    //             StateHasChanged();
    //         }
    //     }
    // }

    public string DisplayNumber => Number?.ToString() ?? string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }

    protected override void OnInitialized()
    {
        if (CellParameter is null) return;

        Row = CellParameter.Row;
        Column = CellParameter.Column;
    }
}
