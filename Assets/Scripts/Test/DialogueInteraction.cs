using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueInteraction : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private AgentController agentController;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TMP_Text dialogueText;

    //==================================================
    // SETTINGS
    //==================================================

    [Header("Settings")]

    [SerializeField]
    private float typingSpeed = 0.03f;

    //==================================================
    // UNITY EVENTS
    //==================================================

    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //==================================================
    // INPUT
    //==================================================

    private void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        ShowPlayerMessage(text);

        agentController.ProcessPlayerInput(text);

        inputField.text = "";

        inputField.ActivateInputField();
    }

    //==================================================
    // UI
    //==================================================

    private void ShowPlayerMessage(string text)
    {
        StopAllCoroutines();

        StartCoroutine(
            TypewriterEffect(
                "Player: " + text));
    }

    public void ShowAgentMessage(string text)
    {
        StopAllCoroutines();

        StartCoroutine(
            TypewriterEffect(
                "Agent: " + text));
    }

    //==================================================
    // TYPEWRITER
    //==================================================

    private IEnumerator TypewriterEffect(string text)
    {
        string currentText = "";

        foreach (char c in text)
        {
            currentText += c;

            dialogueText.text = currentText;

            yield return new WaitForSeconds(
                typingSpeed);
        }
    }
}
