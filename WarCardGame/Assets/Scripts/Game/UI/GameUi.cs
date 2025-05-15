using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    public CardDisplay playerCardDisplay;
    public CardDisplay botCardDisplay;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI roundText;
    public Button drawButton;

    private GameManager _gameManager;

    [SerializeField]
    private SceneLoader _sceneLoader;

    private async void Start()
    {
        var deckService = new DeckService();
        _gameManager = new GameManager(deckService);
        _gameManager.OnRoundCompleted += UpdateUI;
        _gameManager.OnGameEnded += EndGame;

        await _gameManager.StartGameAsync();
        drawButton.onClick.AddListener(async () => await _gameManager.PlayRoundAsync());
    }

    private void UpdateUI(Card playerCard, Card botCard, RoundResult result, int playerScore, int botScore)
    {
        playerCardDisplay.SetCard(playerCard.ImageUrl);
        botCardDisplay.SetCard(botCard.ImageUrl);
        scoreText.text = $"Player: {playerScore} - Bot: {botScore}";
        roundText.text = $"Round: {_gameManager.Round}";
    }

    private void EndGame(string resultMessage)
    {
        PlayerPrefs.SetString("GameResult", resultMessage);
        _sceneLoader.LoadTargetScene("Result");
    }

}
