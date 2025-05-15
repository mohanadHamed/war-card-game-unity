using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ResultUi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _resultText;

    [SerializeField]
    private Button _mainMenuButton;

    [SerializeField]
    private SceneLoader _sceneLoader;

    private void Start()
    {
        _resultText.text = PlayerPrefs.GetString("GameResult", "Game Over");
        _mainMenuButton.onClick.AddListener(() => BackToMenu());
    }

    public void BackToMenu()
    {
        _sceneLoader.LoadTargetScene("MainMenu");
    }
}
