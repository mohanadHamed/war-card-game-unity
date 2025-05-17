namespace Network.DeckService
{
    [System.Serializable]
    public class DeckShuffleResponse
    {
        public bool success;
        public string deck_id;
        public int remaining;
        public bool shuffled;
    }
}
