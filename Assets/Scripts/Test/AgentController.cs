using System;
using System.Collections.Generic;
using UnityEngine;
using static AgentController.ActionResult;
using static Event;

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
    [SerializeField]
    private InventorySystem inventorySystem;
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
    [SerializeField]
    private float observationDisplayDuration = 3f;
    [System.Serializable]
    public class ExecutionContext
    {
        public string Goal;
        public List<string> CompletedTasks = new List<string>();
        public string CurrentTask;
        public string FailedTask;
        public string FailureReason;
    }
    private ExecutionContext executionContext = new ExecutionContext();
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
        public string target;

        public string content;
        public int importance;

        public List<string> plans;

        public string itemName;
    }
    //observation
    private string latestObservation = "";
    private void Start()
    {
        movementController.OnMoveFinished += HandleMoveFinished;
    }
    //actions
    public Action<string> OnThoughtGenerated;
    public void ProcessPlayerInput(string playerInput)
    {
        executionContext = new ExecutionContext();

        executionContext.Goal = playerInput;
        //send observation first time
        ObserveEnvironment(() =>
        {
            List<LLMService.Message> messages = BuildMessages(playerInput);

            llmService.SendRequest(messages, OnLLMResponse);
        });
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
        - you can only use the locations in Known Location.

        Available Game Actions:
        - move
        - observe
        - stay
        - reflect
        - plan
        - pickup
        - use

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

        Pickup Action Format:
        {{
          ""thought"":""The cable may be useful."",
          ""action"":""pickup"",
          ""result"":
          {{
              ""itemName"":""Black Cable""
          }}
        }}

        Use Action Format:
        {{
            ""thought"":""The cable may work here."",
            ""action"":""use"",
            ""result"":
            {{
                ""itemName"":""Black Cable""
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
                    ""move table"",
                    ""pickup black cable"",
                    ""move monitor"",
                    ""use black cable""
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
        - If the user specifies a location, move directly there.
        - Do not visit unrelated locations.
        - Do not explore unless the location is unknown.
        - If an item location is already known, go directly to that location.
        - Use only these task formats:
          move <location>
          pickup <item>
          use <item>
          observe

        - Do not create free-form tasks.
        - Every task must match one of the formats above.

        Current Observation:
        {latestObservation}

        Current Memories:
        {memoryPrompt}

        Current Location:
        {movementController.GetCurrentWayPoint().name}

        Known Location:
        {memorySystem.BuildKnownWayPointPrompt()}

        Items in bag:
        {inventorySystem.BuildItemsPrompt()}";


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
    private void ExecuteAction(AIAction action)
    {
        ContinueAction(action);
    }
    private void ContinueAction(AIAction action)
    {
        Debug.Log("[Thought]:" + action.thought);
        Debug.Log("[Executing Action]:" + action.action);

        switch (action.action)
        {
            case "plan":

                PlanTasks(action.result.plans);

                break;

            case "move":

                if (!MoveToTarget(action.result.target))
                {
                    OnTaskFailed("Target not found");
                }

                break;

            case "observe":

                ObserveEnvironment(() =>
                {
                    FinishCurrentTask();
                });

                break;

            case "stay":

                FinishCurrentTask();

                break;

            case "reflect":

                memorySystem.AddMemory(
                    action.result.content,
                    action.result.importance,
                    MemorySystem.MemoryType.Reflection);

                FinishCurrentTask();

                break;

            case "pickup":
                PickupItem(action.result.itemName);
                break;

            case "use":
                UseItem(action.result.itemName);
                break;

            default:

                OnTaskFailed(
                    "Unknown Action");

                break;
        }
    }
    private void OnTaskFailed(
    string reason)
    {
        executionContext.FailedTask =
            executionContext.CurrentTask;

        executionContext.FailureReason =
            reason;

        StartReplan();
    }
    // ACTIONS
    // MOVE
    private bool MoveToTarget(string target)
    {
        Debug.Log("Moving To: " + target);

        if (!memorySystem.hasWayPoint(target)) return false; 

        if (movementController != null)
        {
            movementController.MoveToNode(target);
        }
        return true;
    }
    [SerializeField]
    private float thoughtReadTime = 2.5f;
    private void HandleMoveFinished()
    {
        Debug.Log("Move Finished");

        Invoke(nameof(StartObserveAfterMove), thoughtReadTime);
    }

    private void StartObserveAfterMove()
    {
        ObserveEnvironment(() =>
        {
            FinishCurrentTask();
        });
    }
    // OBSERVE
    private string lastObservationLocation = "";
    private void ObserveEnvironment(Action onFinished)
    {
        string observationText = observationSystem.BuildObservationSpeech(movementController.GetCurrentWayPoint());

        if (lastObservationLocation == movementController.GetCurrentWayPoint().name)
        {
            onFinished?.Invoke();
            return;
        }
        else 
            lastObservationLocation = movementController.GetCurrentWayPoint().name;
        OnThoughtGenerated?.Invoke(observationText);

        observationSystem.SaveObservationAsMemory(observationText, llmService,
                () =>
                {
                    Invoke(nameof(FinishObservation), observationDisplayDuration);

                    cachedObserveFinished = onFinished;
                });
    }
    private Action cachedObserveFinished;

    private void FinishObservation()
    {
        cachedObserveFinished?.Invoke();
    }
    //Plan
    private void PlanTasks(List<string> plans)
    {
        if (plans == null || plans.Count == 0)
        {
            Debug.LogWarning("No Plans Generated");
            return;
        }

        tasks.Clear();

        tasks.AddRange(plans);

        currentTaskIndex = 0;

        isExecutingTask = false;

        Invoke(nameof(ExecuteNextTask), taskWaitingDuration);
    }
    private List<LLMService.Message>
    BuildReplanMessages()
    {
        string prompt =
        $@"Goal:
        {executionContext.Goal}

        Completed Tasks:
        {string.Join("\n",
            executionContext.CompletedTasks)}

        Failed Task:
        {executionContext.FailedTask}

        Failure Reason:
        {executionContext.FailureReason}

        Current Location:
        {movementController
            .GetCurrentWayPoint()
            .name}

        Inventory:
        {inventorySystem.BuildItemsPrompt()}

        Generate a PLAN action.

        Output JSON only.

        Do not repeat completed tasks.

        Create a new plan from current state.";

        return new List<LLMService.Message>()
        {
            new LLMService.Message()
            {
                role = "system",
                content = BuildSystemPrompt()
            },

            new LLMService.Message()
            {
                role = "user",
                content = prompt
            }
        };
    }
    private void OnReplanResponse(string response, bool success)
    {
        if (!success)
            return;

        AIAction action = ParseAction(response);

        if (action == null)
            return;

        if (action.action != "plan")
            return;

        tasks.Clear();

        tasks.AddRange( action.result.plans);

        currentTaskIndex = 0;

        isExecutingTask = false;

        Invoke(nameof(ExecuteNextTask), taskWaitingDuration);
    }
    //Pickup
    private void PickupItem(string itemName)
    {
        WayPoint current = movementController.GetCurrentWayPoint();

        foreach (Event evt in current.events)
        {
            if (evt.GetEventType() != MyEventType.Pickup)
                continue;

            ItemData item = evt.GetItem();

            if (item.itemName != itemName)
                continue;

            inventorySystem.AddItem(item);

            current.events.Remove(evt);

            Destroy(evt.gameObject);

            FinishCurrentTask();

            return;
        }

        OnTaskFailed("Item not found");
    }
    //Use
    private void UseItem(string itemName)
    {
        if (!inventorySystem.HasItem(itemName))
        {
            OnTaskFailed("Missing item");
            return;
        }

        WayPoint current = movementController.GetCurrentWayPoint();

        foreach (Event evt in current.events)
        {
            if (evt.GetEventType() != MyEventType.Use)
            {
                continue;
            }

            ItemData requiredItem = evt.GetItem();

            if (requiredItem == null)
            {
                continue;
            }

            if (requiredItem.itemName != itemName)
            {
                continue;
            }

            inventorySystem.RemoveItem(itemName);

            memorySystem.AddMemory(evt.GetInteractionResult(), 8);

            evt.stateUpdate("Activated");

            FinishCurrentTask();

            return;
        }

        OnTaskFailed("Cannot use item here");
    }
    //tasks
    private bool ExecuteDeterministicTask(string task)
    {
        task = task.Trim().ToLower();

        if (task.StartsWith("move "))
        {
            string target = task.Replace("move ", "");

            return MoveToTarget(target);
        }

        if (task.StartsWith("pickup "))
        {
            string item = task.Replace("pickup ", "");

            PickupItem(item);

            return true;
        }

        if (task.StartsWith("use "))
        {
            string item = task.Replace("use ", "");

            UseItem(item);

            return true;
        }

        if (task.StartsWith("observe"))
        {
            ObserveEnvironment(() =>
            {
                FinishCurrentTask();
            });

            return true;
        }

        return false;
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

        executionContext.CurrentTask = currentTask;

        Debug.Log("Current Task: " + currentTask);


        if (!ExecuteDeterministicTask(currentTask))
        {
            ProcessTask(currentTask);
        }
        //ProcessTask(currentTask);
    }
    private void StartReplan()
    {
        List<LLMService.Message> messages = BuildReplanMessages();

        llmService.SendRequest(messages, OnReplanResponse);
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
        if (!string.IsNullOrEmpty(executionContext.CurrentTask))
        {
            executionContext.CompletedTasks.Add(executionContext.CurrentTask);
        }

        currentTaskIndex++;

        isExecutingTask = false;

        Invoke(nameof(ExecuteNextTask), taskWaitingDuration);
    }
}
