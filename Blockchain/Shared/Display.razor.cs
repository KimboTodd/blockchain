using System.Diagnostics;

namespace Blockchain.Shared;

[DebuggerDisplay("Row: {Row}, Column: {Column} Number: {Number}")]
public partial class Display : ComponentBase
{
    public Display()
    {
    }

    [Parameter]
    public string Text { get; set; }
}