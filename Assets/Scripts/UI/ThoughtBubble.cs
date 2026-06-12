using System.Collections;
using UnityEngine;
using TMPro;
using System.Xml.Serialization;

public class ThoughtBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private AgentController agentController;
    [SerializeField]
    private TMP_Text dialogueText;
    [SerializeField]
    private SpriteRenderer background;
    [Header("Settings")]
    [SerializeField]
    private float typingSpeed = 0.02f;
    [SerializeField]
    private float fadeDuration = 0.5f;
    [SerializeField]
    private float visibleDuration = 3f;
    [SerializeField]
    [Range(0, 255)]
    private int targetAlpha = 240;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agentController.OnThoughtGenerated += ShowAgentThought;
        Color color = background.color;
        color.a = 0f;
        background.color = color;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
        agentController.OnThoughtGenerated -= ShowAgentThought;
    }
    private Coroutine thoughtRoutine;
    private void ShowAgentThought(string text)
    {
        // STOP PREVIOUS
        if (thoughtRoutine != null)
        {
            StopCoroutine(thoughtRoutine);
        }
        // START NEW
        thoughtRoutine = StartCoroutine(ShowThoughtRoutine(text));
    }
    private IEnumerator ShowThoughtRoutine(string text)
    {
        // SHOW BACKGROUND
        yield return StartCoroutine(FadeBackground(0f, targetAlpha / 255f));
        // TYPEWRITER
        yield return StartCoroutine(TypewriterEffect(text));
        // WAIT
        yield return new WaitForSeconds(visibleDuration);
        // HIDE BACKGROUND
        yield return StartCoroutine(FadeBackground(targetAlpha / 255f, 0f));
        // CLEAR TEXT
        dialogueText.text = "";
    }
    //Fade Effect
    private IEnumerator FadeBackground(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        Color color = background.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);

            background.color = color;

            yield return null;
        }
        // FINAL VALUE
        color.a = endAlpha;
        background.color = color;
    }

    // TYPEWRITER Effect
    private IEnumerator TypewriterEffect(string text)
    {
        string currentText = "";

        foreach (char c in text)
        {
            currentText += c;

            dialogueText.text = currentText;

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
