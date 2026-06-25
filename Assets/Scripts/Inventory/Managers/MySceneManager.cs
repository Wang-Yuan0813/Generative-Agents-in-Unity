using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MySceneManager : MonoBehaviour{

    [Header("Loading UI References")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private CanvasGroup loadingCanvas;

    public static MySceneManager Instance { get; private set; }

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void Initialize(){
        Debug.Log("[MySceneManager] Initialized");
    }

    public void LoadSceneAsync(string sceneName){//start loading a scene asynchronously
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName){
        loadingCanvas.alpha = 1f;   //display loading UI
        loadingCanvas.blocksRaycasts = true;    //block input during loading

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);//start loading the scene
        op.allowSceneActivation = false;//prevent automatic scene activation until we're ready

        while (op.progress < 0.9f){
            float normalized = Mathf.Clamp01(op.progress / 0.9f);
            if (progressBar) progressBar.value = normalized;
            yield return null;
        }

        if (progressBar) progressBar.value = 1f;
        yield return null;

        op.allowSceneActivation = true;//allow the scene to activate

        yield return new WaitForSeconds(0.1f);
        loadingCanvas.alpha = 0f;
        loadingCanvas.blocksRaycasts = false;
    }

}
