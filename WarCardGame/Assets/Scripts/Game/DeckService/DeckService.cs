using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class DeckService
{
    public const string CardBackImageURL = "https://deckofcardsapi.com/static/img/back.png";

    private const int RequestTimeoutSeconds = 5;

    private string _deckId;

    public async UniTask<bool> InitDeckAsync()
    {
        try
        {
            using var request = UnityWebRequest.Get("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
            request.timeout = RequestTimeoutSeconds;
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error initializing deck: " + request.error);
                return false;
            }

            var deckShuffleResponse = JsonUtility.FromJson<DeckShuffleResponse>(request.downloadHandler.text);
            _deckId = deckShuffleResponse.deck_id;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error initializing deck service " + e.Message);
            return false;
        }
    }

    public async UniTask<Card> DrawCardAsync()
    {
        try
        {
            using var request = UnityWebRequest.Get($"https://deckofcardsapi.com/api/deck/{_deckId}/draw/?count=1");
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
