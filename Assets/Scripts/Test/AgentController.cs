using UnityEngine;
using System.Collections.Generic;

public class AgentController : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private LLMService llmService;

    [SerializeField]
    private MovementController movementController;

    //==================================================
    // CHARACTER SETTINGS
    //==================================================

    [Header("Character Settings")]

    [SerializeField]
    private string agentName = "Agent1";

    [TextArea(5, 20)]
    [SerializeField]
    private string personalityPrompt;

    //==================================================
    // AI ACTION
    //==================================================

    [System.Serializable]
    public class AIAction
    {
        public string action;

        public string target;
    }

    //==================================================
    // PUBLIC API
    //==================================================

    /// <summary>
    /// Send player command to AI
    /// </summary>
    public void ProcessPlayerInput(string playerInput)
    {
        List<LLMService.Message> messages =
            BuildMessages(playerInput);

        llmService.SendRequest(
            messages,
            OnLLMResponse);
    }

    //==================================================
    // BUILD PROMPT
    //==================================================

    private List<LLMService.Message> BuildMessages(
        string playerInput)
    {
        string systemPrompt = BuildSystemPrompt();

        List<LLMService.Message> messages =
            new List<LLMService.Message>()
        {
            new LLMService.Message
            {
                role = "system",
                content = systemPrompt
            },

            new LLMService.Message
            {
                role = "user",
                content = playerInput
            }
        };

        return messages;
    }

    //==================================================
    // SYSTEM PROMPT
    //==================================================

    private string BuildSystemPrompt()
    {
        
        string prompt =
        $@"You are {agentName}, an AI agent in a game world.

        You must strictly output JSON.

        Do not output explanations.
        Do not output natural language.

        Character Personality:
        {personalityPrompt}

        Available Actions:
        - move

        JSON Format:
        {{
            ""action"": """",
            ""target"": """"
        }}";
        /*
        string knownLocationText =
            string.Join(", ", knownLocations);

        string visibleObjectText =
            string.Join(", ", visibleObjects);string prompt =
        $@"You are an AI agent in a game world.

        You must strictly output JSON.

        Do not output explanations.
        Do not output natural language.

        Character Personality:
        {personalityPrompt}

        Current Location:
        {currentLocation}

        Known Locations:
        {knownLocationText}

        Visible Objects:
        {visibleObjectText}

        Available Actions:
        - move
        - observe

        JSON Format:
        {{
            ""action"": """",
            ""target"": """"
        }}";*/

        return prompt;
    }

    //==================================================
    // HANDLE RESPONSE
    //==================================================

    private void OnLLMResponse(
        string response,
        bool success)
    {
        if (!success)
        {
            Debug.LogError("LLM Request Failed");
            return;
        }

        Debug.Log("LLM Response:");
        Debug.Log(response);

        AIAction action =
            ParseAction(response);

        if (action == null)
        {
            Debug.LogError("Failed To Parse AI Action");
            return;
        }
        Debug.Log("AI Response:");
        Debug.Log(response);

        ExecuteAction(action);
    }

    //==================================================
    // PARSE ACTION
    //==================================================

    private AIAction ParseAction(string json)
    {
        try
        {
            return JsonUtility.FromJson<AIAction>(json);
        }
        catch
        {
            return null;
        }
    }

    //==================================================
    // EXECUTE ACTION
    //==================================================

    private void ExecuteAction(AIAction action)
    {
        Debug.Log("Executing Action:");
        Debug.Log(action.action);

        switch (action.action)
        {
            case "move":

                MoveToTarget(action.target);

                break;

            case "observe":

                ObserveEnvironment();

                break;

            default:

                Debug.LogWarning(
                    "Unknown Action: " +
                    action.action);

                break;
        }
    }

    //==================================================
    // MOVE
    //==================================================

    private void MoveToTarget(string target)
    {
        Debug.Log("Moving To: " + target);

        if (movementController != null)
        {
            movementController.MoveToNode(target);
        }
    }

    //==================================================
    // OBSERVE
    //==================================================

    private void ObserveEnvironment()
    {
        Debug.Log("Observing Environment...");
    }

    //==================================================
    // MEMORY UPDATE
    //==================================================

    /*public void AddKnownLocation(string location)
    {
        if (!knownLocations.Contains(location))
        {
            knownLocations.Add(location);
        }
    }

    public void AddVisibleObject(string objectName)
    {
        if (!visibleObjects.Contains(objectName))
        {
            visibleObjects.Add(objectName);
        }
    }

    public void SetCurrentLocation(string location)
    {
        currentLocation = location;
    }*/
}