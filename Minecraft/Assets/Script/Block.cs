using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum ItemType
{
    Dirt, Grass, Water,
    Wood, Leaf, Stone, Coal, Iron,
    axe
}

public class Block : MonoBehaviour
{
    [Header("Block Stat")]
    public ItemType type = ItemType.Dirt;

    public int maxHP = 3;

    [HideInInspector] public int hp;

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

