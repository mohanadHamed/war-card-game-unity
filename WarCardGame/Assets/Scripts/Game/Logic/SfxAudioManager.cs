using Game.Interfaces;
using GameDataSave;
using UnityEngine;

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

            _cardFlipAudio = Resources.Load<AudioClip>(CardFlipAudioFileName);
            _winAudio = Resources.Load<AudioClip>(WinAudioFileName);
            _loseAudio = Resources.Load<AudioClip>(LoseAudioFileName);
            _victoryAudio = Resources.Load<AudioClip>(VictoryAudioFileName);
            _gameOverLoseAudio = Resources.Load<AudioClip>(GameOverLoseAudioFileName);
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
    }
}
