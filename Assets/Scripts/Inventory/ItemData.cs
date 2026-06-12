using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;

    [TextArea]
    public string description;
}