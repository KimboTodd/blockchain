namespace Blockchain.Shared;

partial class ScoreKeeper : ComponentBase
{
    private bool _IsPlaying;

    private List<int> _LastLinksBroken = new();

    private bool comboAnimation = false;

    [Parameter]
    public bool IsGameOver { get; set; }

    [Parameter]
    public bool IsPlaying
    {
        get => _IsPlaying;
        set
        {
            if (value == true)
            {
                Score = 0;
            }

            _IsPlaying = value;
        }
    }

    public async Task OnLinksBroken(int linksBroken)
    {
        if (linksBroken == 0)
        {
            _LastLinksBroken.Clear();
        }
        else
        {
            _LastLinksBroken.Add(linksBroken);

            var currentPoints = fibonacciScore(linksBroken) * 10;
            var comboMultiplier = _LastLinksBroken.Count;
            var comboPoints = currentPoints * comboMultiplier;
            Score += comboPoints;
            StateHasChanged();

            if (comboMultiplier > 1)
            {
                comboAnimation = true;
                StateHasChanged();
                await InvokeAsync(async () =>
                {
                    await Task.Delay(1000).ContinueWith(_ => comboAnimation = false);
                    StateHasChanged();
                });
            }
        }
    }

    private int Score { get; set; }

    private int fibonacciScore(int linksBroken)
    {
        if (linksBroken is 0 or 1)
        {
            return linksBroken;
        }

        return fibonacciScore(linksBroken - 1) + fibonacciScore(linksBroken - 2);
    }
}