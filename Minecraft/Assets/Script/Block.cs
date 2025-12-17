using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 및 블록 종류 (확장됨)
public enum ItemType
{
    // 자연 블록
    Dirt, Grass, Water,
    Wood, Leaf, Stone, Coal, Iron, Sand,

    // 가공된 재료
    Plank,  // 판자
    Stick,  // 막대기

    // 도구
    Axe,        // 도끼
    Pickaxe,    // 곡괭이
    Shovel,     // 삽
    Sword       // 검
}

// 도구 타입 정의 (어떤 도구인지)
public enum ToolType
{
    None,       // 도구 아님 / 맨손
    Pickaxe,    // 곡괭이
    Axe,        // 도끼
    Shovel,     // 삽
    Sword       // 검
}

public class Block : MonoBehaviour
{
    [Header("Block Stat")]
    public ItemType type = ItemType.Dirt;

    public int maxHP = 3;

    [HideInInspector] public int hp;

    // [추가됨] 이 블록을 캘 때 효율적인 도구 (예: 돌->곡괭이)
    public ToolType effectiveTool = ToolType.None;

    public int dropCount = 1;

    public bool mineable = true;

    void Awake()
    {
        hp = maxHP;

        // 콜라이더가 없으면 자동으로 추가
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // 태그 설정
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
        {
            gameObject.tag = "Block";
        }
    }

    public void Hit(int damage, Inventory inven)
    {
        if (!mineable) return;

        hp -= damage;
        // 여기에 타격 이펙트나 사운드 재생 코드를 넣을 수 있습니다.

        if (hp <= 0)
        {
            if (inven != null && dropCount > 0)
            {
                inven.Add(type, dropCount);
            }
            Destroy(gameObject);
        }
    }
}