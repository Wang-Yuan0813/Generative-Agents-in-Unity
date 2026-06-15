using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public List<string> validLocations;
    [TextArea]
    public string description;
    [TextArea]
    public string useSuccessResult;

    [TextArea]
    public string useFailResult;
}