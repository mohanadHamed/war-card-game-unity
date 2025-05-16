using System;
using Cysharp.Threading.Tasks;

public class GameManager
{
    public event Action<RoundResult, int, int, int, bool> OnRoundCompleted;
    public event Func<Card, UniTask> OnPlayerCardReady;
    public event Func<Card, UniTask> OnBotCardReady;

    public event Action<string> OnGameEnded;
    public event Action OnGameStarted;
    public event Action<RoundResult> OnShowAwardLabels;
    
    public int Round => _round;

    //private const int MaxRounds = 8;
    private const int ScoreToWin = 8;
    private const int BotDelayMs = 1000;
    private const int DelayToLoadGameOverMs = 2000;
    private const int DelayToStartNextRoundMs = 1000;


    private readonly DeckService _deckService;

    private int _playerScore;
    private int _botScore;
    private int _round;

    private bool isGameOver => _playerScore == ScoreToWin || _botScore == ScoreToWin;
    
    public GameManager(DeckService deckService)
    {
        _deckService = deckService;
    }

    public async UniTask StartGameAsync()
    {
        _playerScore = _botScore = _round = 0;
        OnGameStarted?.Invoke();

        await _deckService.InitDeckAsync();
    }

    public async UniTask PlayRoundAsync()
    {
        if (/*_round >= MaxRounds ||*/ _playerScore == ScoreToWin || _botScore == ScoreToWin)
            return;

        _round++;
        var playerCard = await _deckService.DrawCardAsync();
        await OnPlayerCardReady.Invoke(playerCard);

        await UniTask.Delay(BotDelayMs);

        var botCard = await _deckService.DrawCardAsync();
        await OnBotCardReady.Invoke(botCard);


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

        OnShowAwardLabels?.Invoke(result);

        await UniTask.Delay(DelayToStartNextRoundMs);
        OnRoundCompleted?.Invoke(result, _playerScore, _botScore, _round, isGameOver);
        
        if (isGameOver)
        {
            await GotoGameOver();
        }
    }

    private async UniTask GotoGameOver()
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
