using UnityEngine;
using System.Collections.Generic;
using System;

public class ObservationSystem : MonoBehaviour
{
    [SerializeField]
    private MemorySystem memorySystem;
    
    public List<string> GetObservations(WayPoint currentNode)
    {
        List<string> observations = new List<string>();

        observations.Add($"I am in {currentNode.name}.");

        memorySystem.AddKnownWayPoint(currentNode);

        if (currentNode.events.Count > 0)
        {
            foreach (Event evt in currentNode.events)
            {
                observations.Add(evt.GetObservationText());
            }
        }
        else
        {
            observations.Add("There is nothing interesting here.");
        }

        foreach (WayPoint neighbor in currentNode.neighbors)
        {
            observations.Add($"I can go to {neighbor.name}.");

            memorySystem.AddKnownWayPoint(neighbor);

        }

        return observations;
    }

    public string BuildObservationSpeech(WayPoint currentNode)
    {
        List<string> observations = GetObservations(currentNode);

        List<string> important = new List<string>();

        foreach (string obs in observations)
        {
            if (obs.Contains("nothing interesting"))
                continue;

            if (obs.StartsWith("I can go to"))
                continue;

            if (obs.StartsWith("Pickable Item:"))
                continue;

            if (obs.StartsWith("Usable With:"))
                continue;

            important.Add(obs);
        }

        if (important.Count == 0)
        {
            return $"Nothing interesting in {currentNode.name}.";
        }

        return string.Join(" ", important);
    }

    public void SaveObservationAsMemory(
        string observation,
        LLMService llmService,
        Action onFinished)
    {
        string prompt =
        $@"Convert this observation into a concise memory.

        Rules:
        - Keep it short.
        - Keep important information.
        - Remove unnecessary details.
        - Output only the memory.

        Observation:
        {observation}";

        List<LLMService.Message> messages =
            new()
            {
                new LLMService.Message
                {
                    role = "system",
                    content = prompt
                }
            };

        llmService.SendRequest(
            messages,
            (response, success) =>
            {
                if (success)
                {
                    memorySystem.AddMemory(
                        response.Trim(),
                        3,
                        MemorySystem.MemoryType.Observation);

                    Debug.Log("New Memory: " + response);
                }

                onFinished?.Invoke();
            });
    }
}