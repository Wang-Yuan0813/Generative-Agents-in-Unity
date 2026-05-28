using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LLMDialogueManager dialogueManager;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text dialogueText;

    private string characterName;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    void Start()
    {
        characterName = dialogueManager.npcCharacter.name;

        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void OnSubmit(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        dialogueManager.SendDialogueRequest(text, HandleAIResponse);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void HandleAIResponse(string response, bool success)
    {
        StartCoroutine(TypewriterEffect(
            success
            ? characterName + ": " + response
            : characterName + ": （通讯中断）"
        ));
    }

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