using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Common;
using Game.DataTypes;
using Game.Logic;

namespace Result
{
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
                SfxAudioManager.Instance.PlayVictory();
            }
            else
            {
                SfxAudioManager.Instance.PlayGameOverLose();
            }
        }

        public void BackToMenu()
        {
            _sceneLoader.LoadTargetScene(SceneLoader.MainMenuSceneName);
        }
    }
}
