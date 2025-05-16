using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private TextMeshProUGUI _cardFallbackText;


    private static Sprite _backImageSprite;

    public async UniTask<bool> SetCard(string imageUrl, string namedValue)
    {
        _cardFallbackText.gameObject.SetActive(false);
        await LoadImageAsync(imageUrl, namedValue);
        return true;
    }

    public async UniTask<bool> SetCardToBackImage()
    {
        _cardFallbackText.gameObject.SetActive(false);
        await LoadCardBackImageAsync();
        return true;
    }

    private async UniTask<bool> LoadImageAsync(string url, string namedValue)
    {
        using var request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            _cardFallbackText.text = namedValue;
            _cardFallbackText.gameObject.SetActive(true);
            Debug.LogError($"Failed to load image from {url}: {request.error}");
            return false;
        }

        var texture = DownloadHandlerTexture.GetContent(request);
        _cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
        
        return true;
    }

    private async UniTask<bool> LoadCardBackImageAsync()
    {
        if (_backImageSprite == null)
        {
            // Download texture
            using var request = UnityWebRequestTexture.GetTexture(DeckService.CardBackImageURL);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image from {DeckService.CardBackImageURL}: {request.error}");
                return false;
            }

            var texture = DownloadHandlerTexture.GetContent(request);

            // Apply sprite
            _backImageSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        _cardImage.sprite = _backImageSprite;

        return true;
    }
}
