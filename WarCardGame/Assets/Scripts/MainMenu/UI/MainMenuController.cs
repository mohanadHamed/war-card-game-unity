using Common;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Ui
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _settingsPanel;

        [SerializeField]
        private Button _playButton;
        [SerializeField] 
        private Button _settingsButton;
    
        [SerializeField]
        private SceneLoader _sceneLoader;

        private void Start()
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            _settingsPanel.SetActive(false);
        }

        public void PlayButtonClick()
        {
            DisableAllButtons();
            _sceneLoader.LoadTargetScene(SceneLoader.GameSceneName);
        }

        public void SettingsButtonClick()
        {
            _settingsPanel.SetActive(true);
        }

        public void BackButtonClick()
        {
            InitializeUi();
        }

        private void DisableAllButtons()
        {
            _playButton.interactable = false;
            _settingsButton.interactable = false;
        }
    }
}
