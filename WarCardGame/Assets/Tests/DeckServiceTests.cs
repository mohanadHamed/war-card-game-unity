using NUnit.Framework;
using Network.DeckService;
using System.Threading.Tasks;

public class DeckServiceTests
{
    [Test]
    public async Task InitDeckAsync_ReturnsValidDeckId()
    {
        var deckId = await DeckService.InitDeckAsync();
        Assert.IsFalse(string.IsNullOrEmpty(deckId), "Deck ID should not be null or empty.");
    }

    [Test]
    public async Task DrawCardAsync_ReturnsValidCard()
    {
        var deckId = await DeckService.InitDeckAsync();
        Assume.That(!string.IsNullOrEmpty(deckId), "Failed to initialize deck.");

        var card = await DeckService.DrawCardAsync(deckId);
        Assert.IsNotNull(card, "Card should not be null.");
        Assert.IsFalse(string.IsNullOrEmpty(card.NamedValue), "Card value should not be empty.");
        Assert.IsFalse(string.IsNullOrEmpty(card.ImageUrl), "Card image URL should not be empty.");
    }

    [Test]
    public async Task InitDeckAsync_InvalidUrl_ReturnsNull()
    {
        var badApi = "https://invalid-url.abc";
        var deckId = await DeckService.InitDeckAsync(badApi);
        Assert.IsNull(deckId, "Deck ID should be null for invalid URL.");
    }

    [Test]
    public async Task DrawCardAsync_InvalidDeckId_ReturnsNull()
    {
        var deckId = "nonexistent";
        var card = await DeckService.DrawCardAsync(deckId);

        Assert.IsNull(card, "Card should be null when using invalid deck ID.");
    }

    [Test]
    public async Task InitDeckAsync_ValidUrl_InvalidJson_ReturnsNull()
    {
        var deckId = await DeckService.InitDeckAsync("https://www.googleapis.com/youtube/v3/videos");
        Assert.IsNull(deckId, "Deck ID should be null due to unexpected JSON format.");
    }
}
