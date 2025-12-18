using System.Collections;
using System.Collections.Generic;
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
    public Sprite AxeSprite;
    #endregion

    public List<Transform> SlotItems = new List<Transform>();
    public GameObject SlotItem;
    List<GameObject> Items = new List<GameObject>();

    public int selectedIndex = -1;

    public void UpdateInventory(Inventory myInven)
    {
        // 기존 아이템 오브젝트 삭제
        foreach (var slotItems in Items)
        {
            Destroy(slotItems);
        }
        Items.Clear();

        int idx = 0;
        foreach (var item in myInven.items)
        {
            #region 슬롯 아이템 인스턴스 생성 및 설정
            // 인덱스가 슬롯 개수를 넘어가면 중단 (안전장치)
            if (idx >= SlotItems.Count) break;

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
                case ItemType.Axe:
                    slotItem.ItemSetting(AxeSprite, "x" + item.Value.ToString(), item.Key);
                    break;
            }
            idx++;
        }

        // [오류 수정 핵심] 아이템 목록이 갱신된 후, 선택된 인덱스가 범위를 벗어났는지 확인
        if (selectedIndex >= Items.Count)
        {
            selectedIndex = -1; // 선택 해제
        }

        // 선택 표시 UI 갱신 (선택된 게 있을 때만)
        ResetSelection();
        if (selectedIndex >= 0 && selectedIndex < Items.Count)
        {
            SetSelection(selectedIndex);
        }
    }

    private void Update()
    {
        // Items.Count 범위 내에서만 키 입력 받도록 수정
        for (int i = 0; i < Mathf.Min(9, Items.Count); i++)
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
            if (slot != null && slot.GetComponent<Image>() != null)
                slot.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetSelection(int _idx)
    {
        if (_idx >= 0 && _idx < SlotItems.Count)
        {
            SlotItems[_idx].GetComponent<Image>().color = Color.yellow;
        }
    }

    public ItemType GetInventorySlot()
    {
        // [안전장치 추가] 인덱스가 범위를 벗어나면 기본값 반환
        if (selectedIndex < 0 || selectedIndex >= Items.Count)
        {
            return ItemType.Dirt;
        }
        return Items[selectedIndex].GetComponent<SlotItemPrefab>().blockType;
    }
}