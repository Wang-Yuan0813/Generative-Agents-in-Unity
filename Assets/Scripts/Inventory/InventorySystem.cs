using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        items.Add(item);
    }

    public bool HasItem(string itemName)
    {
        return GetItem(itemName) != null;
    }

    public void RemoveItem(string itemName)
    {
        ItemData item = GetItem(itemName);

        if (item != null)
        {
            items.Remove(item);
        }
    }

    public string BuildItemsPrompt()
    {
        string result = "";

        foreach (var item in items)
        {
            result += item.itemName + "\n";
        }

        return result;
    }
    public ItemData GetItem(string itemName)
    {
        foreach (ItemData item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }
    public ItemData getIndex(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}