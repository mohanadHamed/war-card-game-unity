using Game.Interfaces;
using GameDataSave;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Logic
{
    public class SfxAudioManager : MonoBehaviour, ISfxAudioManager
    {
        public static SfxAudioManager Instance { get; private set; }

        private AudioClip _cardFlipAudio;
        private AudioClip _winAudio;
        private AudioClip _loseAudio;
        private AudioClip _victoryAudio;
        private AudioClip _gameOverLoseAudio;


        private const string CardFlipAudioAddressableKey = "sounds/card_flip";
        private const string WinAudioAddressableKey = "sounds/win";
        private const string LoseAudioAddressableKey = "sounds/lose";
        private const string VictoryAudioAddressableKey = "sounds/victory";
        private const string GameOverLoseAudioAddressableKey = "sounds/game_over_lose";

        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Ensure this object is not destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            Addressables.LoadAssetAsync<AudioClip>(CardFlipAudioAddressableKey).Completed += OnCardFlipAudioLoaded;
            Addressables.LoadAssetAsync<AudioClip>(WinAudioAddressableKey).Completed += OnWinAudioLoaded;
            Addressables.LoadAssetAsync<AudioClip>(LoseAudioAddressableKey).Completed += OnLoseAudioLoaded;
            Addressables.LoadAssetAsync<AudioClip>(VictoryAudioAddressableKey).Completed += OnVictoryAudioLoaded;
            Addressables.LoadAssetAsync<AudioClip>(GameOverLoseAudioAddressableKey).Completed += OnGameOverLoseAudioLoaded;
        }

        public void PlayWin()
        {
            PlaySfxAudio(_winAudio);
        }

        public void PlayLose()
        {
            PlaySfxAudio(_loseAudio);
        }

        public void PlayCardFlip()
        {
            PlaySfxAudio(_cardFlipAudio);
        }

        public void PlayVictory()
        {
            PlaySfxAudio(_victoryAudio);
        }

        public void PlayGameOverLose()
        {
            PlaySfxAudio(_gameOverLoseAudio);
        }

        private void PlaySfxAudio(AudioClip audioClip)
        {
            if (!SaveSystem.Load().SoundEffectsEnabled) return;

            _audioSource.clip = audioClip;
            _audioSource.loop = false;
            _audioSource.Play();
        }

        private void OnCardFlipAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cardFlipAudio = handle.Result;
            }
        }

        private void OnWinAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _winAudio = handle.Result;
            }
        }

        private void OnLoseAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loseAudio = handle.Result;
            }
        }

        private void OnVictoryAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _victoryAudio = handle.Result;
            }
        }

        private void OnGameOverLoseAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _gameOverLoseAudio = handle.Result;
            }
        }
    }
}
