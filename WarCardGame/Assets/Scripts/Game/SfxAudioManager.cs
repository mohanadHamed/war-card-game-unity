using GameDataSave;
using UnityEngine;

namespace Game
{
    public class SfxAudioManager : MonoBehaviour
    {
        public static SfxAudioManager Instance { get; private set; }

        public AudioClip CardFlipAudio { get; private set; }
        public AudioClip WinAudio { get; private set; }
        public AudioClip LoseAudio { get; private set; }
        public AudioClip VictoryAudio { get; private set; }
        public AudioClip GameOverLoseAudio { get; private set; }


        private const string CardFlipAudioFileName = "Sounds/card_flip";
        private const string WinAudioFileName = "Sounds/win";
        private const string LoseAudioFileName = "Sounds/lose";
        private const string VictoryAudioFileName = "Sounds/victory";
        private const string GameOverLoseAudioFileName = "Sounds/game_over_lose";

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

            CardFlipAudio = Resources.Load<AudioClip>(CardFlipAudioFileName);
            WinAudio = Resources.Load<AudioClip>(WinAudioFileName);
            LoseAudio = Resources.Load<AudioClip>(LoseAudioFileName);
            VictoryAudio = Resources.Load<AudioClip>(VictoryAudioFileName);
            GameOverLoseAudio = Resources.Load<AudioClip>(GameOverLoseAudioFileName);
        }

        public void PlaySfxAudio(AudioClip audioClip)
        {
            if (!SaveSystem.Load().SoundEffectsEnabled) return;

            _audioSource.clip = audioClip;
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
}
