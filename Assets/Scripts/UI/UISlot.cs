
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    private int index;
    private InventorySystem inventory;
    [SerializeField]
    private Image namebg;
    [SerializeField]
    private Text itemName;
    [SerializeField]
    private Image icon;
    public void Init(int i, InventorySystem inv)
    {
        index = i;
        inventory = inv;
    }

    public void SetItem(ItemData item)
    {
        if (item != null)
        {
            itemName.text = item.itemName;
            icon.sprite = item.icon;

            icon.enabled = true;
            itemName.enabled = true;
            namebg.enabled = true;

        }
        else
        {

            itemName.text = "";

            icon.enabled = false;
            itemName.enabled = false;
            namebg.enabled = false;

        }
    }
}
