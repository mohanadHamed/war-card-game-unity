using Cysharp.Threading.Tasks;
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

    private GameManager _gameManager;

    private async void Start()
    {
        var deckService = new DeckService();
        _gameManager = new GameManager(deckService);
        _gameManager.OnRoundCompleted += UpdateRoundResultsUi;
        _gameManager.OnPlayerCardReady += UpdatePlayerCardUi;
        _gameManager.OnBotCardReady += UpdateBotCardUi;

        _gameManager.OnGameEnded += EndGame;
        _gameManager.OnGameStarted += UpdateLabels;

        await _gameManager.StartGameAsync();
        _drawButton.onClick.AddListener(async () => await _gameManager.PlayRoundAsync());
    }

    private void UpdatePlayerCardUi(Card playerCard)
    {
        _playerCardDisplay.SetCard(playerCard.ImageUrl, playerCard.NamedValue).Forget();
    }

    private void UpdateBotCardUi(Card botCard)
    {
        _botCardDisplay.SetCard(botCard.ImageUrl, botCard.NamedValue).Forget();
    }

    private void UpdateRoundResultsUi(RoundResult result, int playerScore, int botScore, int round)
    {
        UpdateLabels(playerScore, botScore, round);
    }

    private void UpdateLabels(int playerScore, int botScore, int round)
    {
        _scoreText.text = $"Player: {playerScore} - Bot: {botScore}";
        _roundText.text = $"Round: {round}";
    }

    private void EndGame(string resultMessage)
    {
        PlayerPrefs.SetString("GameResult", resultMessage);
        _sceneLoader.LoadTargetScene("Result");
    }

}
