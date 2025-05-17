using Cysharp.Threading.Tasks;
using Game.DataTypes;
using Game.Interfaces;
using Network.DeckService;

namespace Game.Logic
{
    public class GameDeckService : IGameDeckService
    {
        public async UniTask<string> InitDeckAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return await DeckService.InitDeckAsync();
            }

            return await DeckService.InitDeckAsync(url);
        }

        public async UniTask<Card> DrawCardAsync(string deckId)
        {
            return await DeckService.DrawCardAsync(deckId);
        }
    }
}
