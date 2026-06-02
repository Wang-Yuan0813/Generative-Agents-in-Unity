using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class Event : MonoBehaviour
{
    public Grid grid;
    Vector3 lastPos;

    [Header("Event Setting")]
    [SerializeField]
    [TextArea(5, 20)]
    private string description;
    [SerializeField]
    [TextArea(5, 20)]
    private string currentState;
    void Update()
    {
        if (grid == null) return;
        if (Application.isPlaying) return;

        if (transform.position != lastPos)
        {
            Vector3Int cell = grid.WorldToCell(transform.position);
            Vector3 snappedPos = grid.CellToWorld(cell) + grid.cellSize / 2;

            transform.position = snappedPos;
            lastPos = snappedPos;
        }
    }
    public void stateUpdate(string nextState)
    {
        currentState = nextState;
        Debug.Log(this.name + "has been interacted, state change to->" + nextState);
    }
    
    public string GetObservationText() 
    { 
        return description + "\nCurrent State: " + currentState; 
    }
}
