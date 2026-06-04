using UnityEngine;
using System.Collections.Generic;
using System;

public class ObservationSystem : MonoBehaviour
{
    [SerializeField]
    private MemorySystem memorySystem;
    private Action onObservationFinished;
    // OBSERVATIONS
    public List<string> GetObservations(WayPoint currentNode)
    {
        List<string> observations = new List<string>();
        // Current Location
        observations.Add("Location: " + currentNode.name);
        // Current Node Events
        if(currentNode.events.Count != 0)
            foreach (Event evt in currentNode.events)
                observations.Add(evt.GetObservationText());
        else
            observations.Add("There is nothing interesting in " + currentNode.name);
        // Neighbor Nodes
        foreach (WayPoint neighbor in currentNode.neighbors)
        {
            observations.Add(currentNode.name + "Nearby location: " + neighbor.name);
        }

        return observations;
    }
    // BUILD PROMPT
    public string BuildObservationPrompt(WayPoint wayPoint)
    {
        List<string> observations = GetObservations(wayPoint);

        if (observations.Count == 0)
        {
            return "Nothing interesting nearby.";
        }

        string result = "";

        foreach (string observation in observations)
        {
            result += "- " + observation + "\n";
        }

        return result;
    }
    public void ProcessObservation(string observation, LLMService llmService, Action onFinished)
    {
        string prompt = 
        $@"You are an AI agent memory system.

        Convert the observation into a concise natural language memory.

        Rules:
        - Keep memory short.
        - Keep important information.
        - Infer possible usefulness.
        - Avoid repeating raw observation.
        - Output ONLY the memory sentence.

        Observation:
        {observation}";

        List<LLMService.Message> messages = new List<LLMService.Message>()
        {
            new LLMService.Message
            {
                role = "system",
                content = prompt
            }
        };

        llmService.SendRequest(messages, (response, success) =>
            {
                OnObservationProcessed(response, success, onFinished);
            });
    }
    private void OnObservationProcessed(string response, bool success, Action onFinished)
    {
        if (!success)
        {
            onFinished?.Invoke();
            return;
        }

        string memory = response.Trim();

        memorySystem.AddMemory(memory, 3, MemorySystem.MemoryType.Observation);

        Debug.Log("New Memory: " + memory);

        onFinished?.Invoke();
    }
}
