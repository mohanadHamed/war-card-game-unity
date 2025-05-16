using System;
using Cysharp.Threading.Tasks;

public class GameManager
{
    public event Action<RoundResult, int, int, int, bool> OnRoundCompleted;
    public event Func<Card, UniTask> OnPlayerCardReadyAsync;
    public event Func<Card, UniTask> OnBotCardReadyAsync;
    public event Func<string, UniTask<NetworkNotifyResponse>> OnNetworkIssueNotifyAsync;

    public event Action<string> OnGameEnded;
    public event Action OnQuitToMainMenu;
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

        await InitDeckAsync();
    }

    private async UniTask<bool> InitDeckAsync()
    {
        var success = await _deckService.InitDeckAsync();
        if (!success && OnNetworkIssueNotifyAsync != null)
        {
            var response = await OnNetworkIssueNotifyAsync("A network connection issue occured while initializing deck service");
            if (response == NetworkNotifyResponse.Retry)
            {
                success = await InitDeckAsync();
            }
            else if (response == NetworkNotifyResponse.Quit)
            {
                OnQuitToMainMenu?.Invoke();
            }
        }

        return success;
    }

    public async UniTask PlayRoundAsync()
    {
        if (/*_round >= MaxRounds ||*/ _playerScore == ScoreToWin || _botScore == ScoreToWin)
            return;

        _round++;
        var playerCard = await DrawCardAsync();
        if (OnPlayerCardReadyAsync != null)
        {
            await OnPlayerCardReadyAsync.Invoke(playerCard);
        }

        await UniTask.Delay(BotDelayMs);

        var botCard = await DrawCardAsync();

        if (OnBotCardReadyAsync != null)
        {
            await OnBotCardReadyAsync.Invoke(botCard);
        }


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

    private async UniTask<Card> DrawCardAsync()
    {
        var playerCard = await _deckService.DrawCardAsync();
        if (playerCard == null && OnNetworkIssueNotifyAsync != null)
        {
            var response = await OnNetworkIssueNotifyAsync("A network connection issue occured while drawing a card");
            if (response == NetworkNotifyResponse.Retry)
            {
                playerCard = await DrawCardAsync();
            }
            else if (response == NetworkNotifyResponse.Quit)
            {
                OnQuitToMainMenu?.Invoke();
            }
        }

        return playerCard;
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
