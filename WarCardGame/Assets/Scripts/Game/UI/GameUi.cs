using Cysharp.Threading.Tasks;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    private GameManager _gameManager;

    private async void Start()
    {
        var deckService = new DeckService();
        _gameManager = new GameManager(deckService);
        _gameManager.OnRoundCompleted += UpdateUiAfterRoundCompleted;
        _gameManager.OnPlayerCardReady += UpdatePlayerCardUi;
        _gameManager.OnBotCardReady += UpdateBotCardUi;

        _gameManager.OnGameEnded += EndGame;
        _gameManager.OnGameStarted += ResetUi;
        _gameManager.OnShowAwardLabels += ShowAward;

        await _gameManager.StartGameAsync();
        _drawButton.onClick.AddListener(DrawButtonClick());
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

    private UnityEngine.Events.UnityAction DrawButtonClick()
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

    private void EndGame(string resultMessage)
    {
        PlayerPrefs.SetString("GameResult", resultMessage);
        _sceneLoader.LoadTargetScene("Result");
    }

}
