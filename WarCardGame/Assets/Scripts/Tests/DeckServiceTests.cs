using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;
using Network.DeckService;
using Cysharp.Threading.Tasks;

public class DeckServiceTests
{
    [UnityTest]
    public IEnumerator InitDeckAsync_ReturnsValidDeckId() => UniTask.ToCoroutine(async () =>
    {
        var deckId = await DeckService.InitDeckAsync();
        Assert.IsFalse(string.IsNullOrEmpty(deckId), "Deck ID should not be null or empty.");
    });

    [UnityTest]
    public IEnumerator DrawCardAsync_ReturnsValidCard() => UniTask.ToCoroutine(async () =>
    {
        var deckId = await DeckService.InitDeckAsync();
        Assume.That(!string.IsNullOrEmpty(deckId), "Failed to initialize deck.");

        var card = await DeckService.DrawCardAsync(deckId);
        Assert.IsNotNull(card, "Card should not be null.");
        Assert.IsFalse(string.IsNullOrEmpty(card.NamedValue), "Card value should not be empty.");
        Assert.IsFalse(string.IsNullOrEmpty(card.ImageUrl), "Card image URL should not be empty.");
    });

    [UnityTest]
    public IEnumerator InitDeckAsync_InvalidUrl_ReturnsNull() => UniTask.ToCoroutine(async () =>
    {
        var badApi = "https://invalid-url.abc";
        var deckId = await DeckService.InitDeckAsync(badApi);
        Assert.IsNull(deckId, "Deck ID should be null for invalid URL.");
    });

    [UnityTest]
    public IEnumerator DrawCardAsync_InvalidDeckId_ReturnsNull() => UniTask.ToCoroutine(async () =>
    {
        var deckId = "nonexistent";
        var card = await DeckService.DrawCardAsync(deckId);

        Assert.IsNull(card, "Card should be null when using invalid deck ID.");
    });

    [UnityTest]
    public IEnumerator InitDeckAsync_ValidUrl_InvalidJson_ReturnsNull() => UniTask.ToCoroutine(async () =>
    {
        var deckId = await DeckService.InitDeckAsync("https://www.googleapis.com/youtube/v3/videos");
        Assert.IsNull(deckId, "Deck ID should be null due to unexpected JSON format.");
    });
}
