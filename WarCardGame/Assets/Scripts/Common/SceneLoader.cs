using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common
{
    public class SceneLoader : MonoBehaviour
    {
        public const string MainMenuSceneName = "MainMenu";
        public const string GameSceneName = "Game";
        public const string ResultSceneName = "Result";

        [SerializeField]
        private Slider _loadingBar;

        void Start()
        {
            _loadingBar.gameObject.SetActive(false);
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            _loadingBar.gameObject.SetActive(true);
            _loadingBar.value = 0f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress);
                _loadingBar.value = progress;

                yield return null;
            }
        }

        public void LoadTargetScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }
}
