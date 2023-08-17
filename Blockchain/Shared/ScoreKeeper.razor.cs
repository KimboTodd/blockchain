namespace Blockchain.Shared;

public partial class ScoreKeeper : ComponentBase
{
    private GameState _gameState;

    private List<int>? _lastLinksBroken;

    private bool comboAnimation = false;

    [Parameter]
    public GameState GameState
    {
        get => _gameState;
        set
        {
            if (value != _gameState)
            {
                if (value == GameState.Started)
                {
                    Score = 0;
                    _lastLinksBroken = new List<int>();
                }

                _gameState = value;
            }
        }
    }

    public async Task OnLinksBrokenAsync(int linksBroken)
    {
        _ = _lastLinksBroken ?? throw new Exception(nameof(_lastLinksBroken) + " is null, but should not be");

        if (linksBroken == 0)
        {
            _lastLinksBroken.Clear();
        }
        else
        {
            _lastLinksBroken.Add(linksBroken);

            var currentPoints = FibonacciScore(linksBroken) * 10;
            var comboMultiplier = _lastLinksBroken.Count;
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

    private int FibonacciScore(int linksBroken)
    {
        if (linksBroken is 0 or 1)
        {
            return linksBroken;
        }

        return FibonacciScore(linksBroken - 1) + FibonacciScore(linksBroken - 2);
    }
}