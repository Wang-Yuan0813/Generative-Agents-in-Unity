using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{

    [Header("Scenes")]
    [SerializeField] private string firstScene = "MenuScene";

    [Header("Managers")]
    [SerializeField] private GameObject managersRootPrefab;//root prefab

    [Header("Loading UI (Optional)")]
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private float minimumSplashSeconds = 0.3f;

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        if (managersRootPrefab != null)
        {
            var root = Instantiate(managersRootPrefab);
            DontDestroyOnLoad(root);
        }
        //initialze all manager instances here in order
        GameManager.Instance?.Initialize();
        UIManager.Instance?.Initialize();
        MySceneManager.Instance?.Initialize();
        // SaveManager.Instance?.Load();
        // AudioManager.Instance?.InitMixer();
        // UIManager.Instance?.Init();

        if (loadingCanvas != null)
        {
            loadingCanvas.gameObject.SetActive(true);
        }
        float startTime = Time.realtimeSinceStartup;

        AsyncOperation op = SceneManager.LoadSceneAsync(firstScene, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        float elapsed = Time.realtimeSinceStartup - startTime;
        if (elapsed < minimumSplashSeconds)
            yield return new WaitForSecondsRealtime(minimumSplashSeconds - elapsed);

        op.allowSceneActivation = true;
        yield return null; 

        if (loadingCanvas != null)
        {
            loadingCanvas.enabled = false;
            loadingCanvas.gameObject.SetActive(false);
        }
    }

}
