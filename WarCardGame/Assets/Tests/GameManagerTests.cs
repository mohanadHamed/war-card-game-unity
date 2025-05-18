using NUnit.Framework;
using Cysharp.Threading.Tasks;
using Game.Ui;
using Game.Interfaces;
using Game.DataTypes;
using Game.Logic;
using System.Threading.Tasks;

public class GameManagerTests
{
    private class StubDeckService : IGameDeckService
    {
        public string NextDeckId = "test-deck";
        public Card NextCard = new("url", "5");

        public async UniTask<string> InitDeckAsync(string url)
        {
            return await UniTask.FromResult(NextDeckId);
        }

        public async UniTask<Card> DrawCardAsync(string deckId)
        {
            return await UniTask.FromResult(NextCard);
        }
    }

    private class StubAudioManager : ISfxAudioManager
    {
        public bool PlayedWin = false;
        public bool PlayedLose = false;

        public void PlayWin() => PlayedWin = true;
        public void PlayLose() => PlayedLose = true;
    }

    [Test]
    public async Task StartGameAsync_InitializesDeckAndResetsScores()
    {
        var deckService = new StubDeckService();
        var audioManager = new StubAudioManager();
        var gameManager = new GameManager(deckService, audioManager, GameSettings.Default);

        bool started = false;
        gameManager.OnGameStarted += () =>
        {
            started = true;
            return UniTask.CompletedTask;
        };

        await gameManager.StartGameAsync();

        Assert.IsTrue(started);
        Assert.AreEqual(0, gameManager.Round);
    }

    [Test]
    public async Task PlayRoundAsync_UpdatesScoreAndFiresEvents()
    {
        var deckService = new StubDeckService();
        var audioManager = new StubAudioManager();
        var gameManager = new GameManager(deckService, audioManager, new GameSettings
        {
            ScoreToWin = 0, // force game over
            BotDelayMs = 0,
            DelayToStartNextRoundMs = 0,
            DelayToLoadGameOverMs = 0
        });

        bool roundEnded = false;
        bool gameEnded = false;

        gameManager.OnRoundCompleted += (_, pScore, bScore, round, isOver) =>
        {
            roundEnded = true;
            Assert.AreEqual(0, pScore + bScore);
        };

        gameManager.OnGameEnded += result =>
        {
            gameEnded = true;
        };

        await gameManager.StartGameAsync();
        await gameManager.PlayRoundAsync();

        Assert.IsTrue(roundEnded);
        Assert.IsTrue(gameEnded);
    }

    [Test]
    public async Task PlayRoundAsync_DrawReturnsNull_ShowsNetworkNotify()
    {
        var deckService = new StubDeckService
        {
            NextCard = null // Simulate network fail
        };
        var audioManager = new StubAudioManager();
        var gameManager = new GameManager(deckService, audioManager, GameSettings.Default);

        bool retryPrompted = false;

        gameManager.OnNetworkIssueNotifyAsync += (msg) =>
        {
            retryPrompted = true;
            return UniTask.FromResult(NetworkNotifyResponse.Quit);
        };

        gameManager.OnQuitToMainMenu += () =>
        {
            retryPrompted = true;
        };

        await gameManager.StartGameAsync();
        await gameManager.PlayRoundAsync();

        Assert.IsTrue(retryPrompted);
    }
}
