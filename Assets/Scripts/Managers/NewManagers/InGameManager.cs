using UnityEngine;
using TMPro;

public class InGameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject endingPanel;
    public TMP_Text timer;

    [Header("Timer")]
    public float gameTime = 180f;

    private float currentTime;
    private bool gameEnded = false;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    private void Start()
    {
        currentTime = gameTime;
        UpdateTimerText();
        PlayBGM();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        currentTime -= Time.deltaTime;

        UpdateTimerText();

        if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateTimerText();

            gameEnded = true;
            GameEndLose();
        }
    }
    public void PlayBGM()
    {
        if (bgmSource.isPlaying)
            return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameEndWin()
    {
        gameEnded = true;

        endingPanel.SetActive(true);
        endingPanel.transform.Find("win").gameObject.SetActive(true);
    }

    public void GameEndLose()
    {
        if (!gameEnded)
            return;

        gameEnded = true;

        endingPanel.SetActive(true);
        endingPanel.transform.Find("lose").gameObject.SetActive(true);
    }
}