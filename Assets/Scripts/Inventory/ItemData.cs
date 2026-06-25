using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Items/Item")]
public class ItemData : ScriptableObject
{

    public string itemName;
    public Sprite icon;
    public List<string> validLocations;
    [TextArea]
    public string description;
    [TextArea]
    public string useSuccessResult;

    [TextArea]
    public string useFailResult;
}