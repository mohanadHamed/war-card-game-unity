using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

using Game;

namespace Network.DeckService
{
    public class DeckService
    {
        public const string CardBackImageURL = BaseUrl + "/static/img/back.png";

        private const string BaseUrl = "https://deckofcardsapi.com";
        private const string ApiUrl = BaseUrl + "/api/deck";
        private const string DeckInitUrl = ApiUrl + "/new/shuffle/?deck_count=1";
    
        private const int RequestTimeoutSeconds = 5;

        public static async UniTask<string> InitDeckAsync()
        {
            try
            {
                using var request = UnityWebRequest.Get(DeckInitUrl);
                request.timeout = RequestTimeoutSeconds;
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error initializing deck: " + request.error);
                    return null;
                }

                var deckShuffleResponse = JsonUtility.FromJson<DeckShuffleResponse>(request.downloadHandler.text);
                return deckShuffleResponse.deck_id;
            }
            catch (Exception e)
            {
                Debug.LogError("Error initializing deck service " + e.Message);
                return null;
            }
        }

        public static async UniTask<Card> DrawCardAsync(string deckId)
        {
            try
            {
                var drawCardUrl = $"{ApiUrl}/{deckId}/draw/?count=1";

                using var request = UnityWebRequest.Get(drawCardUrl);
                request.timeout = RequestTimeoutSeconds;
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error drawing card: " + request.error);
                    return null;
                }

                var drawCardResponse = JsonUtility.FromJson<DrawCardResponse>(request.downloadHandler.text);
                var cardData = drawCardResponse.cards[0];
                return new Card(cardData.image, cardData.value);
            }
            catch (Exception e)
            {
                Debug.LogError("Error drawing card: " + e.Message);
                return null;
            }
        }
    }
}
