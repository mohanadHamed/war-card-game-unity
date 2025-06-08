using Common;
using Cysharp.Threading.Tasks;
using Game.DataTypes;
using Game.Interfaces;
using Game.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Game.Ui
{
    public class GameUi : MonoBehaviour
    {
        private const string ButtonBackgroundSpriteAddressableKey = "sprites/circle_button";
        private const string BotSpriteAddressableKey = "sprites/bot";
        private const string DrawButtonSpriteAddressableKey = "sprites/draw_card";

        [SerializeField]
        private CardDisplay _playerCardDisplay;
        [SerializeField]
        private CardDisplay _botCardDisplay;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _roundText;
        [SerializeField]
        private Button _drawButton;
        [SerializeField]
        private Image _botImageBackground;
        [SerializeField]
        private Image _botImageForeground;
        [SerializeField]
        private Image _drawButtonImageBackground;
        [SerializeField]
        private Image _drawButtonImageForeground;

        [SerializeField]
        private SceneLoader _sceneLoader;

        [SerializeField]
        private TextMeshProUGUI _playerAwardText;
        [SerializeField]
        private TextMeshProUGUI _botAwardText;

        [SerializeField]
        private GameObject _networkNotifyPanel;
        [SerializeField]
        private TextMeshProUGUI _networkNotifyMessage;
        [SerializeField]
        private Button _networkRetryButton;
        [SerializeField]
        private Button _networkQuitButton;

        private GameManager _gameManager;

        private NetworkNotifyResponse _networkNotifyReponse;
        private IGameDeckService _gameDeckService;
    
        private async void Start()
        {
            Addressables.LoadAssetAsync<Sprite>(ButtonBackgroundSpriteAddressableKey).Completed += OnButtonImageBackgroundLoaded;
            Addressables.LoadAssetAsync<Sprite>(BotSpriteAddressableKey).Completed += OnBotImageLoaded;
            Addressables.LoadAssetAsync<Sprite>(DrawButtonSpriteAddressableKey).Completed += OnDrawButtonImageLoaded;

            _gameDeckService = new GameDeckService();
            _gameManager = new GameManager(_gameDeckService, SfxAudioManager.Instance, GameSettings.Default);
            _gameManager.OnRoundCompleted += UpdateUiAfterRoundCompleted;
            _gameManager.OnPlayerCardReadyAsync += UpdatePlayerCardUi;
            _gameManager.OnBotCardReadyAsync += UpdateBotCardUi;
            _gameManager.OnNetworkIssueNotifyAsync += ShowNetworkIssueNotify;

            _gameManager.OnGameEnded += EndGame;
            _gameManager.OnQuitToMainMenu += QuiGameToMainMenu;
            _gameManager.OnGameStarted += ResetUi;
            _gameManager.OnShowAwardLabels += ShowAward;

            _drawButton.onClick.AddListener(DrawButtonClick());
            _networkRetryButton.onClick.AddListener(RetryButtonClick);
            _networkQuitButton.onClick.AddListener(QuitButtonClick);

            await _gameManager.StartGameAsync();
        }

        private void QuitButtonClick()
        {
            _networkNotifyReponse = NetworkNotifyResponse.Quit;
        }

        private void RetryButtonClick()
        {
            _networkNotifyReponse = NetworkNotifyResponse.Retry;
        }

        private async UniTask<NetworkNotifyResponse> ShowNetworkIssueNotify(string message)
        {
            _networkNotifyReponse = NetworkNotifyResponse.None;

            _networkNotifyPanel.SetActive(true);
            _networkNotifyMessage.text = message;

            while (_networkNotifyReponse == NetworkNotifyResponse.None)
            {
                await UniTask.DelayFrame(1);
            }

            _networkNotifyPanel.SetActive(false);
            return _networkNotifyReponse;
        }

        private async UniTask ResetUi()
        {
            UpdateScoreBoard(0, 0, 0);
            await SetCardsToBackImage();
            _drawButton.interactable = true;
        }

        private async UniTask SetCardsToBackImage()
        {
            await _playerCardDisplay.SetCardToBackImage();
            await _botCardDisplay.SetCardToBackImage();
        }

        private UnityAction DrawButtonClick()
        {
            return async () =>
            {
                DisableDrawButton();
                await _gameManager.PlayRoundAsync();
            };
        }

        private async UniTask UpdatePlayerCardUi(Card playerCard)
        {
            await _playerCardDisplay.SetCard(playerCard.ImageUrl, playerCard.NamedValue);
        }

        private async UniTask UpdateBotCardUi(Card botCard)
        {
            await _botCardDisplay.SetCard(botCard.ImageUrl, botCard.NamedValue);
        }

        private void UpdateUiAfterRoundCompleted(RoundResult result, int playerScore, int botScore, int round, bool isGameOver)
        {
            UpdateScoreBoard(playerScore, botScore, round);
            HideAwards();

            if (!isGameOver)
            {
                SetCardsToBackImage().Forget();
                EnableDrawButton();
            }
        }

        private void EnableDrawButton()
        {
            _drawButton.interactable = true;
        }

        private void DisableDrawButton()
        {
            _drawButton.interactable = false;
        }

        private void UpdateScoreBoard(int playerScore, int botScore, int round)
        {
            _scoreText.text = $"Player: {playerScore} - Bot: {botScore}";
            _roundText.text = $"Round: {round}";
        }

        private void ShowAward(RoundResult result)
        {
            switch(result)
            {
                case RoundResult.PlayerWins:
                    _playerAwardText.gameObject.SetActive(true);
                    _botAwardText.gameObject.SetActive(false);
                    break;

                case RoundResult.BotWins:
                    _playerAwardText.gameObject.SetActive(false);
                    _botAwardText.gameObject.SetActive(true);
                    break;

                default:
                    break;
            }
        }

        private void HideAwards()
        {
            _playerAwardText.gameObject.SetActive(false);
            _botAwardText.gameObject.SetActive(false);
        }

        private void EndGame(GameResult gameResult)
        {
            PlayerPrefs.SetInt("GameResult", (int)gameResult);
            _sceneLoader.LoadTargetScene(SceneLoader.ResultSceneName);
        }

        private void QuiGameToMainMenu()
        {
            _sceneLoader.LoadTargetScene(SceneLoader.MainMenuSceneName);
        }

        private void OnButtonImageBackgroundLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _botImageBackground.sprite = handle.Result;
                _drawButtonImageBackground.sprite = handle.Result;
            }
        }

        private void OnBotImageLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _botImageForeground.sprite = handle.Result;
            }
        }

        private void OnDrawButtonImageLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _drawButtonImageForeground.sprite = handle.Result;
            }
        }
    }
}
