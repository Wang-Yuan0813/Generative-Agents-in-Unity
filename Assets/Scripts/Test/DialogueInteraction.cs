using System.Collections;
using UnityEngine;
using TMPro;
using System.Xml.Serialization;

public class DialogueInteraction : MonoBehaviour
{
    // REFERENCES
    [Header("References")]
    [SerializeField]
    private AgentController agentController;
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TMP_Text dialogueText;
    [SerializeField]
    private SpriteRenderer background;
    // SETTINGS
    [Header("Settings")]
    [SerializeField]
    private float typingSpeed = 0.03f;
    [SerializeField]
    private float fadeDuration = 0.5f;
    [SerializeField]
    private float visibleDuration = 3f;
    [SerializeField]
    [Range(0, 255)]
    private int targetAlpha = 100;

    // UNITY EVENTS
    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
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
    // INPUT
    private void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        //ShowPlayerMessage(text);

        agentController.ProcessPlayerInput(text);

        inputField.text = "";

        inputField.ActivateInputField();
    }
    // UI
    private void ShowPlayerMessage(string text)
    {
        StopAllCoroutines();

        StartCoroutine(TypewriterEffect("Player: " + text));
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
