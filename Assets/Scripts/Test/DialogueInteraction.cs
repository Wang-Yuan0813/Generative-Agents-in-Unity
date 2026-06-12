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
    private Animator animator;


    // UNITY EVENTS
    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    // INPUT
    private void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        text = "plan:" + text;

        agentController.ProcessPlayerInput(text);

        inputField.text = "";

        //inputField.ActivateInputField();


        animator.SetTrigger("Finish");
    }
}
