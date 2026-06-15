using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class Event : MonoBehaviour
{
    [Header("Event Setting")]
    [SerializeField]
    [TextArea(5, 20)]
    private string description;
    [SerializeField]
    [TextArea(5, 20)]
    private string currentState;
    [SerializeField]
    [TextArea(2, 20)]
    private string userDisplay;

    [Header("Pulse")]

    [SerializeField]
    private bool enablePulse = true;

    [SerializeField]
    private float pulseInterval = 0.2f;

    [SerializeField]
    private float bigScaleMultiplier = 1.2f;

    private Vector3 originalScale;

    private float timer;

    private bool isBig = false;

    public enum MyEventType
    {
        Observation,
        Pickup,
        Use
    }
    [SerializeField]
    private MyEventType eventType;

    [Header("Item")]
    [SerializeField]
    private ItemData item;

    [SerializeField]
    [TextArea]
    private string interactionResult;

    private void Awake()
    {
        originalScale = transform.localScale;
    }
    void Update()
    {
        if (Application.isPlaying && enablePulse)
        {
            timer += Time.deltaTime;

            if (timer >= pulseInterval)
            {
                timer = 0f;

                isBig = !isBig;

                if (isBig)
                {
                    transform.localScale = originalScale * bigScaleMultiplier;
                }
                else
                {
                    transform.localScale = originalScale;
                }
            }
        }
    }
    public void stateUpdate(string nextState)
    {
        currentState = nextState;
        Debug.Log(this.name + "has been interacted, state change to->" + nextState);
    }
    
    /*public string GetObservationText() 
    {
        return description;
    }*/
    public string GetObservationText()
    {
        switch (eventType)
        {
            case MyEventType.Pickup:

                return
                    $"{description}\n" +
                    $"Pickable Item: {item.itemName}";

            case MyEventType.Use:

                return
                    $"{description}\n" +
                    $"Usable With: {item.itemName}";

            default:

                return description;
        }
    }
    public string GetDisplayObservationText()
    {
        return description;
    }
    public MyEventType GetEventType()
    {
        return eventType;
    }
    public ItemData GetItem()
    {
        return item;
    }
    public string GetInteractionResult()
    {
        return interactionResult;
    }
}
