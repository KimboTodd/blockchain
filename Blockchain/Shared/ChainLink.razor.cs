using System.Diagnostics;

namespace Blockchain.Shared;

[DebuggerDisplay("Row: {Row}, Column: {Column} Number: {Number}")]
public partial class ChainLink : ComponentBase
{
    public ChainLink()
    {
    }

    [Parameter]
    public ChainLink? CellParameter { get; set; }

    [Parameter]
    public int? Number { get; set; }

    [Parameter]
    public bool? Scored { get; set; }

    public ChainLink(int row, int column, int number)
    {
        Row = row;
        Column = column;
        Number = number;
    }

    public string DisplayNumber => Number?.ToString() ?? string.Empty;

    public int Row { get; set; }

    public int Column { get; set; }

    protected override void OnInitialized()
    {
        if (CellParameter is null)
        {
            return;
        }

        Row = CellParameter.Row;
        Column = CellParameter.Column;
    }
}