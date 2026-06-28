using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject storyPanel;
    public GameObject tutorialPanel;
    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;
    private bool isTutorial = false;
    void Start()
    {
        storyPanel.SetActive(false);
        PlayBGM();
    }
    public void PlayBGM()
    {
        if (bgmSource.isPlaying)
            return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);

        storyPanel.SetActive(true);
    }
    public void TutorialSwitch()
    {
        isTutorial = !isTutorial;
        tutorialPanel.SetActive(isTutorial);
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
