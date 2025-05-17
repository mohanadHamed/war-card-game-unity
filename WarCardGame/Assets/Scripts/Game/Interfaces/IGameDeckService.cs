using Cysharp.Threading.Tasks;
using Game.DataTypes;

namespace Game.Interfaces
{
    public interface IGameDeckService
    {
        UniTask<string> InitDeckAsync(string url);
        UniTask<Card> DrawCardAsync(string deckId);
    }
}
