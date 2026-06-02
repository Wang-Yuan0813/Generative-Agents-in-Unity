using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    // REFERENCES
    [Header("References")]
    [SerializeField]
    private LLMService llmService;
    [SerializeField]
    private MovementController movementController;
    [SerializeField]
    private MemorySystem memorySystem;
    [SerializeField]
    private ObservationSystem observationSystem;
    // CHARACTER SETTINGS
    [Header("Character Properies")]
    [SerializeField]
    private string agentName = "Agent1";
    [SerializeField]
    [TextArea(2, 20)]
    private string personalityPrompt;
    [SerializeField]
    [TextArea(3, 20)]
    private string memoryPrompt;//save memory as a natural language text
    [SerializeField]
    [TextArea(3, 20)]
    private string observationPrompt;//save observation as a natural language text
    [SerializeField]
    private Queue<string> tasks;//tasks queue
    // AI ACTION
    [System.Serializable]
    public class AIAction
    {
        public string thought;
        public string action;
        public ActionResult result;
    }
    [System.Serializable]
    public class ActionResult
    {
        //move action
        public string target;
        //reflect action
        public string content; 
        public int importance;
    }
    //actions
    public Action<string> OnThoughtGenerated;
    /// <summary>
    /// Send player command to AI
    /// </summary>
    public void ProcessPlayerInput(string playerInput)
    {
        List<LLMService.Message> messages = BuildMessages(playerInput);

        llmService.SendRequest(messages, OnLLMResponse);
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

        Format:

        {{
            ""thought"":"""",
            ""action"":"""",
            ""result"":{{}}
        }}

        Rules:
        - thought must be short.
        - thought should explain the reason for the action.
        - action must be a game action.
        - result format depends on action type.

        Available Game Actions:
        - move
        - observe
        - stay
        - reflect
        - plan

        Move Action Format:

        {{
            ""thought"":""The kitchen may contain food."",
            ""action"":""move"",
            ""result"":
            {{
                ""target"":""kitchen""
            }}
        }}

        Reflect Action Format:

        {{
            ""thought"":""I have enough memories to infer a pattern."",
            ""action"":""reflect"",
            ""result"":
            {{
                ""content"":""The player seems interested in food."",
                ""importance"":8
            }}
        }}

        Rules for Reflection:
        - content must summarize multiple memories.
        - content must be high-level knowledge.
        - importance must be between 1 and 10.
        - do not repeat raw observations.
        - reflection should infer personality, preference, habit, or world knowledge.

        Current Memories:
        {memoryPrompt}";

        /*string prompt =
        $@"You are {agentName}, an AI agent in a game world. 
        You must strictly output JSON. 
        Do not output explanations. 
        Format: 
        {{ 
            ""thought"":"""", 
            ""action"":"""", 
            ""result"":"""" 
        }} 
        Rules: 
        - thought must be short. 
        - thought should explain the reason for the action. 
        - action must be a game action. 
        - result must be a waypoint or object name or a action result. 
        Available Game Actions:
        -move
        -observe
        -stay
        -reflect
        -plan
        Move Example: 
        {{ 
            ""thought"":""The kitchen may contain food."", 
            ""action"":""move"", 
            ""result"":""kitchen"" 
        }}
        Reflect Example:
        {{ 
            ""thought"":""I have too much memories. It's time to organize my thoughts."", 
            ""action"":""reflect"", 
            ""result"":
            {{
                ""content"":""The player seems interested in food."",
                ""importance"":8
            }}  
        }}
        Current Memories:
        {memoryPrompt}";*/

        return prompt;
    }

    //==================================================
    // HANDLE RESPONSE
    //==================================================

    private void OnLLMResponse(string response, bool success)
    {
        if (!success)
        {
            Debug.LogError("LLM Request Failed");
            return;
        }

        Debug.Log("LLM Response:");
        Debug.Log(response);

        AIAction action = ParseAction(response);

        if (action == null)
        {
            Debug.LogError("Failed To Parse AI Action");
            return;
        }
        OnThoughtGenerated?.Invoke(action.thought);//invoke action for UI display
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
        //before every action, observe the environment first
        ObserveEnvironment();
        Debug.Log("current observation prompt:\n" + observationPrompt + "\n=======");
        Debug.Log("current memory prompt:\n" + memoryPrompt + "\n=======");

        Debug.Log("[Thought]:" + action.thought);
        Debug.Log("[Executing Action]:" + action.action);
        switch (action.action)
        {
            case "move":
                MoveToTarget(action.result.target);
                break;
            case "observe":
                break;
            case "stay":
                break;
            case "reflect":
                memorySystem.AddMemory(action.result.content, action.result.importance, MemorySystem.MemoryType.Reflection);
                break;
            case "plan":
                break;
            default:
                Debug.LogWarning("Unknown Action: " + action.action);
                break;
        }
        
    }
    // ACTIONS
    // MOVE
    private void MoveToTarget(string target)
    {
        Debug.Log("Moving To: " + target);

        if (movementController != null)
        {
            movementController.MoveToNode(target);
        }
    }
    // OBSERVE
    private void ObserveEnvironment()
    {
        Debug.Log("Observing Environment...");
        observationPrompt = observationSystem.BuildObservationPrompt(movementController.GetCurrentWayPoint());//get observation from this location
        memorySystem.AddMemory(observationPrompt);//convert observation into memory
        memoryPrompt = memorySystem.BuildMemoryPrompt();
        Debug.Log("Observing done!");
    }

}