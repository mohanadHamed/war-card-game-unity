
using System.Collections.Generic;

namespace Game
{
    public class Card
    {
        public string ImageUrl { get; private set; }
        public string NamedValue { get; private set; }
        public int Value { get; private set; }

        private readonly Dictionary<string, int> valueMap = new Dictionary<string, int>()
        {
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
            { "10", 10 },
            { "jack", 11 },
            { "queen", 12 },
            { "king", 13 },
            { "ace", 14 },
        };

        public Card(string imageUrl, string namedValue)
        {
            ImageUrl = imageUrl;
            NamedValue = namedValue;
            Value = valueMap[namedValue.ToLower()];
        }
    }
}
