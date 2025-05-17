using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
using Network.DeckService;

namespace Game.Ui
{
    public class CardDisplay : MonoBehaviour
    {
        [SerializeField] private Image _cardImage;
        [SerializeField] private TextMeshProUGUI _cardFallbackText;

        private static Sprite _backImageSprite;

        public async UniTask<bool> SetCard(string imageUrl, string namedValue)
        {
            _cardFallbackText.gameObject.SetActive(false);
            await ShowCardWithFlip(imageUrl, namedValue);
            return true;
        }

        public async UniTask<bool> SetCardToBackImage()
        {
            _cardFallbackText.gameObject.SetActive(false);
            await LoadCardBackImageAsync();
            return true;
        }

        private async UniTask<bool> ShowCardWithFlip(string url, string namedValue)
        {
            SfxAudioManager.Instance.PlaySfxAudio(SfxAudioManager.Instance.CardFlipAudio);
        
            await FlipOutCard();

            using var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                await FlipInCard();
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

            await FlipInCard();

            return true;
        }

        private async UniTask FlipInCard()
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_cardImage.transform.DORotate(Vector3.zero, 0.1f));
            await sequence.AsyncWaitForCompletion();
        }

        private async UniTask FlipOutCard()
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_cardImage.transform.DORotate(new Vector3(0, 90, 180), 0.2f));
            await sequence.AsyncWaitForCompletion();
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
}
