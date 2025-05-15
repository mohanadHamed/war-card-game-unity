using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

        Initialize();
    }

    private void Start()
    {
        // Play background music
        if (_bgmAudioSource != null)
        {
            _bgmAudioSource.loop = true;
            UpdateBgmVolume();
            _bgmAudioSource.Play();
        }
    }

    public void UpdateBgmVolume()
    {
        _bgmAudioSource.volume = SaveSystem.Load().SoundMusicEnabled ? 1f : 0;
    }

    private void Initialize()
    {
        _bgmAudioSource = GetComponent<AudioSource>();
    }
}
