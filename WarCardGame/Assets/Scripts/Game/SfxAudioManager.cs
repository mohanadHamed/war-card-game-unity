using UnityEngine;

public class SfxAudioManager : MonoBehaviour
{
    public static SfxAudioManager Instance { get; private set; }

    public AudioClip CardFlipAudio => _cardFlipAudio;
    public AudioClip WinAudio => _winAudio;
    public AudioClip LoseAudio => _loseAudio;
    public AudioClip VictoryAudio => _victoryAudio;
    public AudioClip GameOverLoseAudio => _gameOverLoseAudio;


    [SerializeField]
    private AudioClip _cardFlipAudio;
    [SerializeField]
    private AudioClip _winAudio;
    [SerializeField]
    private AudioClip _loseAudio;
    [SerializeField]
    private AudioClip _victoryAudio;
    [SerializeField]
    private AudioClip _gameOverLoseAudio;

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
    }

    public void PlaySfxAudio(AudioClip audioClip)
    {
        if (!SaveSystem.Load().SoundEffectsEnabled) return;

        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
