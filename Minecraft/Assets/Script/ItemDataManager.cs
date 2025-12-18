using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    // 어디서든 접근할 수 있게 싱글톤 인스턴스 설정
    public static ItemDataManager Instance;

    [Header("Data List")]
    // 인스펙터에서 만든 ItemData(곡괭이 데이터 등)를 여기에 드래그해서 등록하세요
    public List<ItemData> itemDataList;

    // 내부적으로 빠르게 찾기 위한 사전(Dictionary)
    private Dictionary<ItemType, ItemData> itemDictionary = new Dictionary<ItemType, ItemData>();

    void Awake()
    {
        Instance = this;

        // 리스트에 등록된 데이터를 딕셔너리로 정리
        foreach (var data in itemDataList)
        {
            if (data != null && !itemDictionary.ContainsKey(data.itemType))
            {
                itemDictionary.Add(data.itemType, data);
            }
        }
    }

    // 외부(PlayerHarvester 등)에서 아이템 정보를 물어볼 때 쓰는 함수
    public ItemData GetItemData(ItemType type)
    {
        if (itemDictionary.TryGetValue(type, out var data))
            return data;

        return null; // 데이터가 없으면 null 반환
    }
}