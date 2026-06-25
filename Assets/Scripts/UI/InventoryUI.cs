
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySystem inventory;
    public GameObject slotPrefab;
    public Transform gridParent;

    private List<UISlot> slots = new List<UISlot>();

    [SerializeField]
    private int displayNum = 0;
    void Start()
    {
        for (int i = 0; i < displayNum; i++)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            UISlot slot = obj.GetComponent<UISlot>();

            slot.Init(i, inventory);
            slots.Add(slot);
        }

        RefreshUI();
    }

    void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetItem(inventory.getIndex(i));
        }
    }
}

