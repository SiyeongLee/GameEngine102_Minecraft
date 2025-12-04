using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
    InventoryUI invenUI;

    void Start()
    {
        invenUI = FindObjectOfType<InventoryUI>();
    }
    public int GetCount(ItemType id)
    {
        items.TryGetValue(id, out var count);
        return count;

    }


    public void Add(ItemType type, int count = 1)
    {
        if (!items.ContainsKey(type)) items[type] = 0;
        items[type] += count;
        Debug.Log($"[inven] +{count} {type} (개수 {items[type]})");
        invenUI.UpdateInventory(this);
    }

    public bool Consume(ItemType type, int count = 1)
    {
        if (!items.TryGetValue(type, out var have) || have < count) return false;
        items[type] = have - count;
        Debug.Log($"[inven] -{count} {type} (개수 {items[type]})");

        if (items[type] == 0)
        {
            items.Remove(type);
            invenUI.selectedIndex = -1;
            invenUI.ResetSelection();
        }

        invenUI.UpdateInventory(this);
        return true;
    }
}