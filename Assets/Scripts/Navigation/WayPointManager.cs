using UnityEngine;
using System.Collections.Generic;

public class WayPointManager : MonoBehaviour
{
    public List<WayPoint> allWayPoints = new List<WayPoint>();

    [ContextMenu("Collect Waypoints")]
    public void CollectWayPoints()
    {
        allWayPoints.Clear();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("WayPoint");

        foreach (var obj in objs)
        {
            WayPoint wp = obj.GetComponent<WayPoint>();

            if (wp != null)
            {
                allWayPoints.Add(wp);
            }
        }
        Debug.Log("Collected " + allWayPoints.Count + " waypoints");

        //sort
        allWayPoints.Sort((a, b) =>
        {
            int numA = ExtractNumber(a.name);
            int numB = ExtractNumber(b.name);
            return numA.CompareTo(numB);
        });

    }
    int ExtractNumber(string name)
    {
        string number = "";

        foreach (char c in name)
        {
            if (char.IsDigit(c))
            {
                number += c;
            }
        }

        if (int.TryParse(number, out int result))
        {
            return result;
        }

        return 0;
    }

    public List<WayPoint> FindPath(WayPoint start, WayPoint target)
    {
        Queue<WayPoint> queue = new Queue<WayPoint>();
        HashSet<WayPoint> visited = new HashSet<WayPoint>();
        Dictionary<WayPoint, WayPoint> parent = new Dictionary<WayPoint, WayPoint>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            WayPoint current = queue.Dequeue();

            if (current == target)
            {
                return ReconstructPath(start, target, parent);
            }

            foreach (var neighbor in current.neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return null;
    }

    List<WayPoint> ReconstructPath(WayPoint start, WayPoint target, Dictionary<WayPoint, WayPoint> parent)
    {
        List<WayPoint> path = new List<WayPoint>();

        WayPoint current = target;

        while (current != start)
        {
            path.Add(current);
            current = parent[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }


    [ContextMenu("Rename All Points")]
    void RenamePoints()
    {
        for (int i = 0; i < allWayPoints.Count; i++) { 
            WayPoint current = allWayPoints[i];
            current._textMeshPro.text = current.name;
        }
    }

}
