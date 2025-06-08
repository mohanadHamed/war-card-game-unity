using GameDataSave;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MainMenu
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance { get; private set; }

        private AudioSource _bgmAudioSource;

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
            _bgmAudioSource = GetComponent<AudioSource>();
            Addressables.LoadAssetAsync<AudioClip>("sounds/bgm").Completed += OnBgmAudioLoaded;
        }

        public void UpdateBgmVolume()
        {
            _bgmAudioSource.volume = SaveSystem.Load().SoundMusicEnabled ? 1 : 0;
        }

        void OnBgmAudioLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Play background music
                if (_bgmAudioSource != null)
                {
                    _bgmAudioSource.clip = handle.Result;
                    _bgmAudioSource.loop = true;
                    UpdateBgmVolume();
                    _bgmAudioSource.Play();
                }
            }
        }
    }
}
