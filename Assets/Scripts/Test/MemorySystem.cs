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
}
