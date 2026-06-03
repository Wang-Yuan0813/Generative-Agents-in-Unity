using UnityEngine;
using System.Collections.Generic;

public class MovementController : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private WayPointManager wayPointManager;

    //==================================================
    // MOVEMENT SETTINGS
    //==================================================

    [Header("Movement Settings")]

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private WayPoint startPoint;

    //==================================================
    // RUNTIME DATA
    //==================================================

    private List<WayPoint> currentPath = new List<WayPoint>();

    private int currentPathIndex = 0;

    private WayPoint currentWayPoint;

    private bool isMoving = false;

    //==================================================
    // UNITY EVENTS
    //==================================================

    private void Start()
    {
        InitializePosition();
    }

    private void Update()
    {
        MoveAlongPath();
    }

    //==================================================
    // INITIALIZE
    //==================================================

    private void InitializePosition()
    {
        if (wayPointManager == null)
        {
            Debug.LogError("WayPointManager Missing");
            return;
        }

        if (startPoint == null)
        {
            Debug.LogError("Start Point Missing");
            return;
        }

        transform.position = startPoint.transform.position;

        currentWayPoint = startPoint;
    }

    //==================================================
    // PUBLIC MOVE API
    //==================================================

    public void MoveToNode(string targetNodeName)
    {
        WayPoint targetNode = FindWayPointByName(targetNodeName);

        if (targetNode == null)
        {
            Debug.LogWarning("Target Node Not Found: " + targetNodeName);
            return;
        }

        if (targetNode == currentWayPoint)
        {
            Debug.Log("Already At Target Node");

            return;
        }

        currentPath = wayPointManager.FindPath(currentWayPoint, targetNode);

        currentPathIndex = 0;

        isMoving = true;

        Debug.Log("Start Moving To: " + targetNodeName);
    }

    //==================================================
    // MOVE
    //==================================================

    private void MoveAlongPath()
    {
        if (!isMoving)
        {
            return;
        }

        if (currentPath == null ||
            currentPath.Count == 0)
        {
            StopMovement();

            return;
        }

        if (currentPathIndex >= currentPath.Count)
        {
            StopMovement();

            return;
        }

        WayPoint targetNode = currentPath[currentPathIndex];

        transform.position =
            Vector3.MoveTowards(transform.position, targetNode.transform.position, moveSpeed * Time.deltaTime);

        float distance =
            Vector3.Distance(transform.position, targetNode.transform.position);

        if (distance < 0.05f)
        {
            ReachNode(targetNode);
        }
    }

    //==================================================
    // NODE REACHED
    //==================================================

    private void ReachNode(WayPoint node)
    {
        currentWayPoint = node;

        //Debug.Log("Reached Node: " + node.name);

        currentPathIndex++;

        if (currentPathIndex >= currentPath.Count)
        {
            StopMovement();
        }
    }

    //==================================================
    // STOP
    //==================================================

    private void StopMovement()
    {
        isMoving = false;

        Debug.Log("Movement Complete");
    }

    //==================================================
    // FIND NODE
    //==================================================

    private WayPoint FindWayPointByName(string nodeName)
    {
        foreach (WayPoint node in wayPointManager.allWayPoints)
        {
            if (node.name.ToLower() == nodeName.ToLower())
            {
                return node;
            }
        }
        Debug.Log("node doesn't exist!");
        return null;
    }

    //==================================================
    // GETTERS
    //==================================================

    public WayPoint GetCurrentWayPoint()
    {
        return currentWayPoint;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}