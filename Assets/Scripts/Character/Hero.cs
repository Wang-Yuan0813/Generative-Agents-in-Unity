using UnityEngine;
using System.Collections.Generic;

public class Hero : MonoBehaviour
{
    public WayPoint startPoint;
    public WayPoint targetPoint;
    /*public WayPointManager wayPointManager;
    public float speed = 5f;
    
    private List<WayPoint> path = new List<WayPoint>();
    private int currentIndex = 0;
    private WayPoint currentPoint;

    public LLMService llm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (wayPointManager.allWayPoints.Count == 0)
        {
            Debug.LogWarning("WayPoints List is empty!");
            return;
        }
        transform.position = startPoint.transform.position;//spawn
        currentPoint = startPoint;
    }

    // Update is called once per frame

    void Update()
    {
        HandleInput();
        MoveAlongPath();
    }

    void HandleInput()
    {
        for (int i = 0; i < wayPointManager.allWayPoints.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                WayPoint target = wayPointManager.allWayPoints[i];
                if (target == currentPoint || target == null) return;

                Debug.Log("go to: " + target.name);

                path = wayPointManager.FindPath(currentPoint, target);
                currentIndex = 0;
            }
        }
    }
    void MoveAlongPath()
    {
        if (path == null || path.Count == 0) return;
        if (currentIndex >= path.Count) return;

        WayPoint targetNode = path[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetNode.transform.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.05f)
        {
            currentPoint = targetNode;

            Debug.Log("reached: " + targetNode.name);

            currentIndex++;
        }
    }*/

}
