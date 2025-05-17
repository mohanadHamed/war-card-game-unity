using System;

namespace Network.DeckService
{
    [Serializable]
    public class CardData
    {
        public string code;
        public string image;
        public CardImages images;
        public string value;
        public string suit;
    }
}
