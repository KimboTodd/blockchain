namespace Blockchain.Client.Shared;

partial class Cell : ComponentBase
{
    [Parameter]
    public int Number { get; set; } = 0;

    public string DisplayNumber => Number.ToString();
    int Row { get; set; }
    int Column { get; set; }
    string CSS { get; set; } = "";
}
