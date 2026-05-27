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


    [ContextMenu("Test Path 1 -> 5")]
    void TestPath()
    {
        if (allWayPoints.Count < 5)
        {
            Debug.LogError("Not enough waypoints!");
            return;
        }

        WayPoint start = allWayPoints[0]; // node1
        WayPoint target = allWayPoints[4]; // node5

        List<WayPoint> path = FindPath(start, target);

        if (path == null)
        {
            Debug.Log("can not reach");
        }
        else
        {
            Debug.Log("size of path:" + path.Count);

            string pathStr = "";
            foreach (var p in path)
            {
                pathStr += p.name + " -> ";
            }

            Debug.Log("path: " + pathStr + "stop");
        }
    }

}
