using System.Collections.Generic;
using UnityEngine;

// 인스펙터에서 아이템 종류와 프리팹을 연결하기 위한 구조체
[System.Serializable]
public struct ItemDropData
{
    public ItemType type;
    public GameObject prefab;
}

public class ItemDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    public List<ItemDropData> dropList; // 여기에 프리팹 등록
    public float throwForce = 5f;       // 던지는 힘

    private Inventory inventory;
    private InventoryUI inventoryUI;
    private Transform throwPoint;       // 던지는 위치 (카메라)

    void Start()
    {
        inventory = GetComponent<Inventory>();
        inventoryUI = FindObjectOfType<InventoryUI>();

        // 메인 카메라를 찾아서 던지는 기준점으로 삼음
        if (Camera.main != null)
            throwPoint = Camera.main.transform;
        else
            throwPoint = transform;
    }

    void Update()
    {
        // Q키 입력 감지
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    void Drop()
    {
        // 1. 현재 선택된 아이템 확인 (인벤토리 UI가 없거나 선택 안 했으면 리턴)
        if (inventoryUI == null || inventoryUI.selectedIndex < 0) return;

        ItemType currentType = inventoryUI.GetInventorySlot();

        // 2. 인벤토리에서 아이템 1개 감소 시도
        if (inventory.Consume(currentType, 1))
        {
            // 3. 해당하는 프리팹 찾기
            GameObject prefab = GetPrefabByType(currentType);

            if (prefab != null)
            {
                SpawnDroppedItem(prefab);
            }
            else
            {
                Debug.LogWarning($"[ItemDropper] {currentType}에 해당하는 프리팹이 등록되지 않았습니다!");
            }
        }
    }

    void SpawnDroppedItem(GameObject prefab)
    {
        // 카메라 앞쪽에서 생성
        Vector3 spawnPos = throwPoint.position + throwPoint.forward * 1.0f;

        // 아이템 생성 (회전은 무작위)
        GameObject droppedItem = Instantiate(prefab, spawnPos, Random.rotation);

        // [중요] 떨어진 아이템은 물리 효과를 받아야 함 (Rigidbody 추가)
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if (rb == null) rb = droppedItem.AddComponent<Rigidbody>();

        // [중요] 아이템처럼 보이게 크기를 좀 줄임
        droppedItem.transform.localScale = Vector3.one * 0.4f;

        // 앞으로 던지는 힘 가하기
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
    }

    // 리스트에서 타입에 맞는 프리팹 찾기
    GameObject GetPrefabByType(ItemType type)
    {
        foreach (var data in dropList)
        {
            if (data.type == type) return data.prefab;
        }
        return null;
    }
}