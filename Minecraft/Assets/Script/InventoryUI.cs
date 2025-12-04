using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    #region Cube Item Sprites
    public Sprite dirtSprite;
    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite WoodSprite;
    public Sprite LeafSprite;
    public Sprite StoneSprite;
    public Sprite CoalSprite;
    public Sprite IronSprite;
    public Sprite axeSprite;
   
    #endregion

    public List<Transform> SlotItems = new List<Transform>();
    public GameObject SlotItem;
    List<GameObject> Items = new List<GameObject>();

    public int selectedIndex = -1;

    public void UpdateInventory(Inventory myInven)
    {
        foreach (var slotItems in Items)
        {
            Destroy(slotItems);
        }
        Items.Clear();

        int idx = 0;
        foreach (var item in myInven.items)
        {
            #region 슬롯 아이템 인스턴스 생성 및 설정
            var go = Instantiate(SlotItem, SlotItems[idx].transform);
            go.transform.localPosition = Vector3.zero;
            SlotItemPrefab slotItem = go.GetComponent<SlotItemPrefab>();
            Items.Add(go);
            #endregion

            switch (item.Key)
            {
                case ItemType.Dirt:
                    slotItem.ItemSetting(dirtSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Grass:
                    slotItem.ItemSetting(grassSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Water:
                    slotItem.ItemSetting(waterSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Wood:
                    slotItem.ItemSetting(WoodSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Leaf:
                    slotItem.ItemSetting(LeafSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Iron:
                    slotItem.ItemSetting(IronSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Stone:
                    slotItem.ItemSetting(StoneSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.Coal:
                    slotItem.ItemSetting(CoalSprite, "x" + item.Value.ToString(), item.Key);
                    break;
                case ItemType.axe:
                    slotItem.ItemSetting(axeSprite, "x" + item.Value.ToString(), item.Key);
                    break;
            }
            idx++;
        }
    }

    private void Update()
    {
        for (int i = 0; i < Mathf.Min(9, SlotItems.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetSelectedIndex(i);
            }
        }
    }

    public void SetSelectedIndex(int idx)
    {
        ResetSelection();

        if (selectedIndex == idx)
        {
            selectedIndex = -1;
        }
        else
        {
            if (idx >= Items.Count)
            {
                selectedIndex = -1;
            }
            else
            {
                SetSelection(idx);
                selectedIndex = idx;
            }
        }
    }

    public void ResetSelection()
    {
        foreach (var slot in SlotItems)
        {
            slot.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetSelection(int _idx)
    {
        SlotItems[_idx].GetComponent<Image>().color = Color.yellow;
    }

    public ItemType GetInventorySlot()
    {
        return Items[selectedIndex].GetComponent<SlotItemPrefab>().blockType;
    }
}