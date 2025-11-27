using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<BlockType, int> items = new Dictionary<BlockType, int>();
    InventoryUI invenUI;

    void Start()
    {
        invenUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        // [조합 기능 구현]

        // 숫자 1번 키: 흙 5개 -> 곡괭이 1개 제작
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (items.ContainsKey(BlockType.Dirt) && items[BlockType.Dirt] >= 5)
            {
                Consume(BlockType.Dirt, 5);
                Add(BlockType.Pickaxe, 1);
                Debug.Log("제작 성공: 곡괭이 획득!");
            }
            else
            {
                Debug.Log("제작 실패: 흙이 5개 필요합니다.");
            }
        }

        // 숫자 2번 키: 나무 5개 -> 무기 1개 제작
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (items.ContainsKey(BlockType.Wood) && items[BlockType.Wood] >= 5)
            {
                Consume(BlockType.Wood, 5);
                Add(BlockType.Weapon, 1);
                Debug.Log("제작 성공: 무기 획득!");
            }
            else
            {
                Debug.Log("제작 실패: 나무가 5개 필요합니다.");
            }
        }
    }

    public void Add(BlockType type, int count = 1)
    {
        if (!items.ContainsKey(type)) items[type] = 0;
        items[type] += count;

        Debug.Log($"[Inven] +{count} {type} (보유량: {items[type]})");

        if (invenUI) invenUI.UpdateInventory(this);
    }

    public bool Consume(BlockType type, int count = 1)
    {
        if (!items.TryGetValue(type, out var have) || have < count) return false;

        items[type] = have - count;
        Debug.Log($"[Inven] -{count} {type} (남은량: {items[type]})");

        if (items[type] == 0)
        {
            items.Remove(type);
            if (invenUI)
            {
                invenUI.selectedIndex = -1;
                invenUI.ResetSelection();
            }
        }

        if (invenUI) invenUI.UpdateInventory(this);
        return true;
    }
}