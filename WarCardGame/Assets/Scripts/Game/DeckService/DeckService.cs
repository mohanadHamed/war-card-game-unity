using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class DeckService
{
    private string _deckId;

    public async UniTask InitDeckAsync()
    {
        using var request = UnityWebRequest.Get("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error initializing deck: " + request.error);
            return;
        }

        var deckShuffleResponse = JsonUtility.FromJson<DeckShuffleResponse>(request.downloadHandler.text);
        _deckId = deckShuffleResponse.deck_id;
    }

    public async UniTask<Card> DrawCardAsync()
    {
        using var request = UnityWebRequest.Get($"https://deckofcardsapi.com/api/deck/{_deckId}/draw/?count=1");
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
}
