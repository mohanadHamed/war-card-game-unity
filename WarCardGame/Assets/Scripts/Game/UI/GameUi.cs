using Cysharp.Threading.Tasks;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
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

    private NetworkNotifyResponse _NetworkNotifyReponse;
    
    private async void Start()
    {
        var deckService = new DeckService();
        _gameManager = new GameManager(deckService);
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
        _NetworkNotifyReponse = NetworkNotifyResponse.Quit;
    }

    private void RetryButtonClick()
    {
        _NetworkNotifyReponse = NetworkNotifyResponse.Retry;
    }

    private async UniTask<NetworkNotifyResponse> ShowNetworkIssueNotify(string message)
    {
        _NetworkNotifyReponse = NetworkNotifyResponse.None;

        _networkNotifyPanel.SetActive(true);
        _networkNotifyMessage.text = message;

        while (_NetworkNotifyReponse == NetworkNotifyResponse.None)
        {
            await UniTask.DelayFrame(1);
        }

        _networkNotifyPanel.SetActive(false);
        return _NetworkNotifyReponse;
    }

    private void ResetUi()
    {
        UpdateScoreBoard(0, 0, 0);
        SetCardsToBackImage();
    }

    private void SetCardsToBackImage()
    {
        _playerCardDisplay.SetCardToBackImage().Forget();
        _botCardDisplay.SetCardToBackImage().Forget();
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
            SetCardsToBackImage();
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
        _sceneLoader.LoadTargetScene("Result");
    }

    private void QuiGameToMainMenu()
    {
        _sceneLoader.LoadTargetScene("MainMenu");
    }

}
