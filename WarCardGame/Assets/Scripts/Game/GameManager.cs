using System;
using Cysharp.Threading.Tasks;

public class GameManager
{
    public event Action<RoundResult, int, int, int> OnRoundCompleted;
    public event Action<Card> OnPlayerCardReady;
    public event Action<Card> OnBotCardReady;

    public event Action<string> OnGameEnded;
    public Action<int, int, int> OnGameStarted;

    //private const int MaxRounds = 8;
    private const int ScoreToWin = 8;
    private const int BotDelayMs = 1000;
    private const int DelayToLoadGameOverMs = 2000;

    private readonly DeckService _deckService;

    private int _playerScore;
    private int _botScore;
    private int _round;
    
    public int Round => _round;

    public GameManager(DeckService deckService)
    {
        _deckService = deckService;
    }

    public async UniTask StartGameAsync()
    {
        _playerScore = _botScore = _round = 0;
        OnGameStarted?.Invoke(_playerScore, _botScore, _round);

        await _deckService.InitDeckAsync();
    }

    public async UniTask PlayRoundAsync()
    {
        if (/*_round >= MaxRounds ||*/ _playerScore == ScoreToWin || _botScore == ScoreToWin)
            return;

        _round++;
        var playerCard = await _deckService.DrawCardAsync();
        OnPlayerCardReady?.Invoke(playerCard);

        await UniTask.Delay(BotDelayMs);

        var botCard = await _deckService.DrawCardAsync();
        OnBotCardReady?.Invoke(botCard);


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

        OnRoundCompleted?.Invoke(result, _playerScore, _botScore, _round);

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
