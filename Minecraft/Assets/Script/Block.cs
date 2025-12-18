using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 목록 (필요한 아이템이 있다면 여기에 콤마(,)로 구분해서 추가하세요)
public enum ItemType
{
    Dirt, Grass, Water,
    Wood, Leaf, Stone, Coal, Iron,
    Axe
    // 예: Pickaxe, Stick, Plank 등을 추가해야 조합법에서 쓸 수 있습니다.
}

public class Block : MonoBehaviour
{
    [Header("Block Stat")]
    public ItemType type = ItemType.Dirt;

    public int maxHP = 3; // 블록 체력

    [HideInInspector] public int hp;

    public int dropCount = 1; // 부서지면 나오는 개수

    public bool mineable = true; // 캘 수 있는 블록인지

    void Awake()
    {
        hp = maxHP;

        // 콜라이더가 없으면 자동으로 추가 (충돌 처리용)
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // 태그가 없으면 자동으로 Block으로 설정
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
        {
            gameObject.tag = "Block";
        }
    }

    // 플레이어가 때렸을 때 호출되는 함수
    public void Hit(int damage, Inventory inven)
    {
        if (!mineable) return;

        hp -= damage;
        // 여기에 타격 이펙트나 사운드 재생 코드를 넣을 수 있습니다.

        if (hp <= 0)
        {
            // 인벤토리가 있고 드롭 개수가 0보다 크면 아이템 획득
            if (inven != null && dropCount > 0)
            {
                inven.Add(type, dropCount);
            }
            Destroy(gameObject); // 블록 파괴
        }
    }
}