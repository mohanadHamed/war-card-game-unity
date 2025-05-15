using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField]
    private Toggle _soundMusicToggle;
    [SerializeField]
    private Toggle _soundEffectsToggle;

    void Start()
    {
        _soundMusicToggle.isOn = SaveSystem.Load().SoundMusicEnabled;
        _soundEffectsToggle.isOn = SaveSystem.Load().SoundEffectsEnabled;

        _soundMusicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        _soundEffectsToggle.onValueChanged.AddListener(OnSoundEffectsToggleChanged);
    }

    public void OnMusicToggleChanged(bool isOn)
    {
        var data = SaveSystem.Load();
        data.SoundMusicEnabled = isOn;
        SaveSystem.Save(data);

        GameManager.Instance.UpdateBgmVolume();
    }

    public void OnSoundEffectsToggleChanged(bool isOn)
    {
        var data = SaveSystem.Load();
        data.SoundEffectsEnabled = isOn;
        SaveSystem.Save(data);
    }
}
