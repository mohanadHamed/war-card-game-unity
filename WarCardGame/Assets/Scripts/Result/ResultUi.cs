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
        var gameResult = (GameResult)PlayerPrefs.GetInt("GameResult");
        _resultText.text = gameResult == GameResult.PlayerWins ? "You Win!" : "Bot Wins!";
        _mainMenuButton.onClick.AddListener(() => BackToMenu());

        if(gameResult == GameResult.PlayerWins )
        {
            SfxAudioManager.Instance.PlaySfxAudio(SfxAudioManager.Instance.VictoryAudio);
        }
        else
        {
            SfxAudioManager.Instance.PlaySfxAudio(SfxAudioManager.Instance.GameOverLoseAudio);
        }
    }

    public void BackToMenu()
    {
        _sceneLoader.LoadTargetScene(SceneLoader.MainMenuSceneName);
    }
}
