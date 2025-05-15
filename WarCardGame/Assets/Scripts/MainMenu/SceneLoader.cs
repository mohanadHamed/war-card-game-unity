using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    [SerializeField]
    private float _sceneTransitionDurationInSeconds = 10f;

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
        operation.allowSceneActivation = false;
        var startTime = Time.time;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            var timedProgress = Mathf.Clamp01((Time.time - startTime) / _sceneTransitionDurationInSeconds);
            _loadingBar.value = Mathf.Min(progress, timedProgress);

            if (_loadingBar.value >= 0.98f)
            {
                // Optional delay before activation
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void LoadTargetScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
}
