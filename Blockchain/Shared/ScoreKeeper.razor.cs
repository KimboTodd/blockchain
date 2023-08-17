namespace Blockchain.Shared;

partial class ScoreKeeper : ComponentBase
{
    private GameState _gameState;

    private List<int>? _LastLinksBroken;

    private bool comboAnimation = false;

    [Parameter]
    public GameState gameState
    {
        get => _gameState;
        set
        {
            if (value != _gameState)
            {
                if (value == GameState.Started)
                {
                    Score = 0;
                    _LastLinksBroken = new List<int>();
                }

                _gameState = value;
            }
        }
    }

    public async Task OnLinksBrokenAsync(int linksBroken)
    {
        _ = _LastLinksBroken ?? throw new Exception(nameof(_LastLinksBroken) + " is null, but should not be");

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