using System;
using Cysharp.Threading.Tasks;
using Game.DataTypes;
using Game.Interfaces;
using Game.Ui;

namespace Game.Logic
{
    public class GameManager
    {
        public event Action<RoundResult, int, int, int, bool> OnRoundCompleted;
        public event Func<Card, UniTask> OnPlayerCardReadyAsync;
        public event Func<Card, UniTask> OnBotCardReadyAsync;
        public event Func<string, UniTask<NetworkNotifyResponse>> OnNetworkIssueNotifyAsync;
        public event Func<UniTask> OnGameStarted;
        public event Action<GameResult> OnGameEnded;
        public event Action OnQuitToMainMenu;
        public event Action<RoundResult> OnShowAwardLabels;

        public int Round => _round;

        private readonly IGameDeckService _gameDeckService;
        private readonly ISfxAudioManager _audioManager;
        private readonly GameSettings _settings;

        private int _playerScore;
        private int _botScore;
        private int _round;

        private string _deckId;

        public GameManager(IGameDeckService deckService, ISfxAudioManager audioManager, GameSettings settings)
        {
            _gameDeckService = deckService;
            _audioManager = audioManager;
            _settings = settings;
        }

        private bool IsGameOver => _playerScore == _settings.ScoreToWin || _botScore == _settings.ScoreToWin;

        public async UniTask StartGameAsync()
        {
            _deckId = await InitDeckAsync();
            _playerScore = _botScore = _round = 0;

            if (OnGameStarted != null)
            {
                await OnGameStarted.Invoke();
            }
        }

        private async UniTask<string> InitDeckAsync()
        {
            var deckId = await _gameDeckService.InitDeckAsync(null);
            if (string.IsNullOrEmpty(deckId) && OnNetworkIssueNotifyAsync != null)
            {
                var response = await OnNetworkIssueNotifyAsync("A network connection issue occurred while shuffling deck");
                if (response == NetworkNotifyResponse.Retry)
                    return await InitDeckAsync();

                if (response == NetworkNotifyResponse.Quit)
                    OnQuitToMainMenu?.Invoke();
            }
            return deckId;
        }

        public async UniTask PlayRoundAsync()
        {
            if (IsGameOver)
            {
                OnRoundCompleted?.Invoke(RoundResult.Draw, 0, 0, 1, true);
                OnGameEnded?.Invoke(GameResult.None);
                return;
            }

            _round++;

            var playerCard = await DrawCardAsync();
            if (OnPlayerCardReadyAsync != null)
                await OnPlayerCardReadyAsync(playerCard);

            await UniTask.Delay(_settings.BotDelayMs);

            var botCard = await DrawCardAsync();
            if (OnBotCardReadyAsync != null)
                await OnBotCardReadyAsync(botCard);

            var result = CompareCards(playerCard, botCard);
            OnShowAwardLabels?.Invoke(result);

            await UniTask.Delay(_settings.DelayToStartNextRoundMs);
            OnRoundCompleted?.Invoke(result, _playerScore, _botScore, _round, IsGameOver);

            if (IsGameOver)
                await GotoGameOver();
        }

        private async UniTask<Card> DrawCardAsync()
        {
            var card = await _gameDeckService.DrawCardAsync(_deckId);
            if (card == null && OnNetworkIssueNotifyAsync != null)
            {
                var response = await OnNetworkIssueNotifyAsync("A network connection issue occurred while drawing a card");
                if (response == NetworkNotifyResponse.Retry)
                    return await DrawCardAsync();

                if (response == NetworkNotifyResponse.Quit)
                    OnQuitToMainMenu?.Invoke();
            }
            return card;
        }

        private RoundResult CompareCards(Card playerCard, Card botCard)
        {
            if (playerCard == null || botCard == null) return RoundResult.Draw;

            if (playerCard.Value > botCard.Value)
            {
                _playerScore++;
                _audioManager.PlayWin();
                return RoundResult.PlayerWins;
            }

            if (playerCard.Value < botCard.Value)
            {
                _botScore++;
                _audioManager.PlayLose();
                return RoundResult.BotWins;
            }

            return RoundResult.Draw;
        }

        private async UniTask GotoGameOver()
        {
            await UniTask.Delay(_settings.DelayToLoadGameOverMs);
            var result = _playerScore == _settings.ScoreToWin ? GameResult.PlayerWins : GameResult.BotWins;
            OnGameEnded?.Invoke(result);
        }
    }
}
