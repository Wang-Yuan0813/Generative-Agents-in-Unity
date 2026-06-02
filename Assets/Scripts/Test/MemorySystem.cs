using NUnit.Framework;
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
}
