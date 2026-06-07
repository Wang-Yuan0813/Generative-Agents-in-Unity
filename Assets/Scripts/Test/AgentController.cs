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
    private List<string> tasks;//tasks queue
    [SerializeField]
    private int currentTaskIndex = 0;
    [SerializeField]
    private float taskWaitingDuration = 5.0f;
    private bool isExecutingTask = false;
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
        //plan
        public List<string> plans;
    }
    private void Start()
    {
        movementController.OnMoveFinished += HandleMoveFinished;
    }
    //actions
    public Action<string> OnThoughtGenerated;
    public void ProcessPlayerInput(string playerInput)
    {
        List<LLMService.Message> messages = BuildMessages(playerInput);

        llmService.SendRequest(messages, OnLLMResponse);
    }
    // BUILD PROMPT
    private List<LLMService.Message> BuildMessages(string playerInput)
    {
        string systemPrompt = BuildSystemPrompt();

        List<LLMService.Message> messages = new List<LLMService.Message>()
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
    // SYSTEM PROMPT
    private string BuildSystemPrompt()
    {
        memoryPrompt = memorySystem.BuildMemoryPrompt();
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

        Plan Action Format:

        {{
            ""thought"":""I should break the task into steps."",
            ""action"":""plan"",
            ""result"":
            {{
                ""plans"":
                [
                    ""move to kitchen"",
                    ""observe kitchen"",
                    ""move to table"",
                    ""take food""
                ]
            }}
        }}

        Rules for Planning:
        - use ""plan"" when the player gives a complex task.
        - plans must be ordered step-by-step tasks.
        - each plan should be short and simple.
        - plans should use game actions.
        - avoid unnecessary steps.
        - maximum 10 plans.

        Current Memories:
        {memoryPrompt}

        Current Location:
        {movementController.GetCurrentWayPoint().name}";

        return prompt;
    }
    // HANDLE RESPONSE
    private void OnLLMResponse(string response, bool success)
    {
        if (!success)
        {
            Debug.LogError("LLM Request Failed");
            return;
        }

        Debug.Log("LLM Response:" + response);

        AIAction action = ParseAction(response);

        if (action == null)
        {
            Debug.LogError("Failed To Parse AI Action");
            return;
        }
        OnThoughtGenerated?.Invoke(action.thought);//invoke action for UI display
        ExecuteAction(action);
    }
    // PARSE ACTION
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
    // EXECUTE ACTION
    private void ExecuteAction(AIAction action)
    {
        //before every action, observe the environment first
        ObserveEnvironment(() =>
        {
            ContinueAction(action);
        });
    }
    private void ContinueAction(AIAction action)
    {
        Debug.Log("[Thought]:" + action.thought);
        Debug.Log("[Executing Action]:" + action.action);
        switch (action.action)
        {
            case "move":
                MoveToTarget(action.result.target);
                break;
            case "observe":
                FinishCurrentTask();
                break;
            case "stay":
                FinishCurrentTask();
                break;
            case "reflect":
                Debug.Log("Reflecting");
                memorySystem.AddMemory(action.result.content, action.result.importance, MemorySystem.MemoryType.Reflection);
                FinishCurrentTask();
                break;
            case "plan":
                Debug.Log("planning");
                PlanTasks(action.result.plans);
                //FinishCurrentTask();
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
    private void HandleMoveFinished() 
    { 
        Debug.Log("Move Finished");
        FinishCurrentTask();
    }
    // OBSERVE
    private void ObserveEnvironment(Action onFinished)
    {
        Debug.Log("Observing Environment...");

        observationPrompt = observationSystem.BuildObservationPrompt(movementController.GetCurrentWayPoint());

        observationSystem.ProcessObservation(observationPrompt, llmService, () =>
            {
                Debug.Log("Observing done!");
                onFinished?.Invoke();
            });
    }

    private void PlanTasks(List<string> plans)
    {
        // CLEAR
        tasks.Clear();
        // ADD
        tasks.AddRange(plans);
        // RESET
        currentTaskIndex = 0;
        // START
        isExecutingTask = false;
        Invoke(nameof(ExecuteNextTask), taskWaitingDuration);
    }
    private void ExecuteNextTask()
    {
        // already running
        if (isExecutingTask)
        {
            return;
        }

        // finished
        if (currentTaskIndex >= tasks.Count)
        {
            Debug.Log("All Tasks Finished");
            return;
        }

        isExecutingTask = true;

        string currentTask = tasks[currentTaskIndex];

        Debug.Log("Current Task: " + currentTask);

        ProcessTask(currentTask);
    }
    private void ProcessTask(string task)
    {
        // BUILD MESSAGE
        List<LLMService.Message> messages = BuildMessages(task);
        // SEND
        llmService.SendRequest(messages, OnTaskResponse);
    }
    private void OnTaskResponse(string response, bool success)
    {
        // FAILED
        if (!success)
        {
            Debug.LogError("Task Request Failed");
            return;
        }
        // PARSE
        AIAction action = ParseAction(response);

        if (action == null)
        {
            Debug.LogError("Failed To Parse Task");
            return;
        }
        // SHOW THOUGHT
        OnThoughtGenerated?.Invoke(action.thought);
        // EXECUTE
        ExecuteAction(action);
    }
    private void FinishCurrentTask()
    {
        currentTaskIndex++;

        isExecutingTask = false;

        Invoke(nameof(ExecuteNextTask), taskWaitingDuration);
    }
}
//plan: You now need to go to point 1 to investigate any clues about where the key might be, and then go to the location of the key.