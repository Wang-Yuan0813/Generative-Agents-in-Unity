using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class WayPoint : MonoBehaviour
{
    public Grid grid;
    /*[SerializeField]
    private WayPointManager wayPointManager = null;*/
    Vector3 lastPos;

    public List<WayPoint> neighbors = new List<WayPoint>();

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
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    public void AddNeighbor(WayPoint other)//add neighbor both
    {

        if (other == null || other == this) return;

        if (!neighbors.Contains(other))
            neighbors.Add(other);

        if (!other.neighbors.Contains(this))
            other.neighbors.Add(this);

    }

    void OnValidate()//execute when data changed
    {
        /*if (wayPointManager == null)
            wayPointManager = GetComponentInParent<WayPointManager>();*/
        foreach (var n in neighbors)
        {
            if (n != null && !n.neighbors.Contains(this))
            {
                n.neighbors.Add(this);
            }
        }
    }

}
