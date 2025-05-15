using System;
using Cysharp.Threading.Tasks;

public class GameManager
{
    public event Action<Card, Card, RoundResult, int, int> OnRoundCompleted;
    public event Action<string> OnGameEnded;

    private int _playerScore;
    private int _botScore;
    private int _round;
    //private const int MaxRounds = 8;
    private const int ScoreToWin = 8;
    private const int BotDelayMs = 1000;
    private const int DelayToLoadGameOverMs = 2000;

    private readonly DeckService _deckService;

    public int Round => _round;

    public GameManager(DeckService deckService)
    {
        _deckService = deckService;
    }

    public async UniTask StartGameAsync()
    {
        _playerScore = _botScore = _round = 0;
        await _deckService.InitDeckAsync();
    }

    public async UniTask PlayRoundAsync()
    {
        if (/*_round >= MaxRounds ||*/ _playerScore == 8 || _botScore == 8)
            return;

        _round++;
        var playerCard = await _deckService.DrawCardAsync();
        await UniTask.Delay(1000);
        var botCard = await _deckService.DrawCardAsync();

        RoundResult result;

        if (playerCard.Value > botCard.Value)
        {
            _playerScore++;
            result = RoundResult.PlayerWins;
        }
        else if (playerCard.Value < botCard.Value)
        {
            _botScore++;
            result = RoundResult.BotWins;
        }
        else
        {
            result = RoundResult.Draw;
        }

        OnRoundCompleted?.Invoke(playerCard, botCard, result, _playerScore, _botScore);

        await CheckGameOver();

    }

    private async UniTask CheckGameOver()
    {
        if (_playerScore == ScoreToWin)
        {
            await UniTask.Delay(DelayToLoadGameOverMs);
            OnGameEnded?.Invoke("Player Wins!");
        }
        else if (_botScore == ScoreToWin)
        {
            await UniTask.Delay(DelayToLoadGameOverMs);
            OnGameEnded?.Invoke("Bot Wins!");
        }
    }
}
