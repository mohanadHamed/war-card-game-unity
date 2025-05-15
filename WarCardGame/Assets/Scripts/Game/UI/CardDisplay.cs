using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    public void SetCard(string imageUrl)
    {
        LoadImageAsync(imageUrl).Forget(); // fire-and-forget since it's non-blocking
    }

    private async UniTaskVoid LoadImageAsync(string url)
    {
        using var request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load image from {url}: {request.error}");
            return;
        }

        var texture = DownloadHandlerTexture.GetContent(request);
        cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}
