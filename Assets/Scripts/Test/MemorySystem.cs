using System;
using System.Collections.Generic;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    // MEMORY TYPE
    public enum MemoryType
    {
        Observation,
        Reflection,
        Plan
    }
    // MEMORY RECORD
    [System.Serializable]
    public class MemoryRecord
    {
        // CONTENT
        public string content;
        // METADATA
        public float timestamp;

        public int importance;

        public MemoryType memoryType;
    }
    // MEMORY STORAGE
    [Header("Memories")]
    [SerializeField]
    private List<MemoryRecord> memories = new List<MemoryRecord>();
    // KNOWN LOCATIONS
    [Header("Known WayPoints")]
    [SerializeField]
    private List<WayPoint> knownWayPoints = new List<WayPoint>();
    // ADD MEMORY
    /*public void AddMemory(string content, int importance = 1, MemoryType type = MemoryType.Observation)
    {
        // CREATE MEMORY
        MemoryRecord memory = new MemoryRecord();

        memory.content = content;

        memory.importance = importance;

        memory.timestamp = Time.realtimeSinceStartup;

        memory.memoryType = type;
        // SAVE
        memories.Add(memory);
        // DEBUG
        Debug.Log($"[{memory.memoryType}] " + $"[{memory.importance}] " + $"{memory.content}");
    }*/
    public void AddMemory(string content, int importance = 1, MemoryType type = MemoryType.Observation)
    {
        // DUPLICATE CHECK
        if (HasRecentMemory(content))
        {
            return;
        }
        // CREATE MEMORY
        MemoryRecord memory = new MemoryRecord();

        memory.content = content;

        memory.importance = importance;

        memory.timestamp = Time.realtimeSinceStartup;

        memory.memoryType = type;
        // SAVE
        memories.Add(memory);
    }

    public bool HasRecentMemory(string content, float recentTime = 30f)
    {
        foreach (MemoryRecord memory in memories)
        {
            // SAME CONTENT
            if (memory.content == content)
            {
                // RECENT
                float deltaTime =
                    Time.realtimeSinceStartup
                    - memory.timestamp;

                if (deltaTime <= recentTime)
                {
                    return true;
                }
            }
        }
        return false;
    }
    // WAYPOINT MEMORY
    public void AddKnownWayPoint(WayPoint wayPoint)
    {
        if (knownWayPoints.Contains(wayPoint))
        {
            return;
        }

        knownWayPoints.Add(wayPoint);
    }
    // RECENT MEMORIES
    public List<MemoryRecord> GetRecentMemories(int count = 5)
    {
        List<MemoryRecord> result = new List<MemoryRecord>();

        int startIndex = Mathf.Max(0, memories.Count - count);

        for (int i = startIndex; i < memories.Count; i++)
        {
            result.Add(memories[i]);
        }

        return result;
    }
    // IMPORTANT MEMORIES
    public List<MemoryRecord> GetImportantMemories(int minImportance = 5)
    {
        List<MemoryRecord> result = new List<MemoryRecord>();

        foreach (MemoryRecord memory in memories)
        {
            if (memory.importance >= minImportance)
            {
                result.Add(memory);
            }
        }

        return result;
    }
    // BUILD MEMORY PROMPT
    public string BuildMemoryPrompt(int recentCount = 10)
    {
        List<MemoryRecord> recent = GetRecentMemories(recentCount);

        string memoryText = "";

        foreach (MemoryRecord memory in recent)
        {
            memoryText += $"[{memory.memoryType}] " + memory.content + "\n";
        }

        return memoryText;
    }
    // REFLECTION CHECK
    public bool ShouldReflect()
    {
        // SIMPLE VERSION
        if (memories.Count >= 20)
        {
            return true;
        }

        return false;
    }
}

/*using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    [System.Serializable]
    public class MemoryRecord
    {
        public string content;
        public float timestamp;
        public int importance;
    }
    [SerializeField] private List<MemoryRecord> memories = new List<MemoryRecord>();
    [SerializeField] private List<WayPoint> knownWayPoints = new List<WayPoint>();
    public void addMemory(string content, int importance = 1)
    {
        //create a memory
        MemoryRecord curmem = new MemoryRecord();
        curmem.content = content;
        curmem.importance = importance;
        curmem.timestamp = Time.realtimeSinceStartup;
        //add to list
        memories.Add(curmem);
        Debug.Log($@"[{curmem.importance}][{curmem.timestamp}]:{curmem.content}\n");
    }
    public void addKnownWayPoint(WayPoint wayPoint)
    {
        if (knownWayPoints.Contains(wayPoint)) return;
        knownWayPoints.Add(wayPoint);
    }
    public List<MemoryRecord> GetRecentMemories(int count = 5) 
    { 
        List<MemoryRecord> result = new List<MemoryRecord>(); 
        int startIndex = Mathf.Max(0, memories.Count - count); 
        for (int i = startIndex; i < memories.Count; i++) 
        { 
            result.Add(memories[i]); 
        } 
        return result; 
    }
    public string BuildMemoryPrompt(int count = 5) 
    { 
        List<MemoryRecord> recentMemories = GetRecentMemories(count); 
        string memoryText = ""; 
        foreach (MemoryRecord memory in recentMemories) 
        { 
            memoryText += memory.content + "\n"; 
        } 
        return memoryText; 
    }
}*/
