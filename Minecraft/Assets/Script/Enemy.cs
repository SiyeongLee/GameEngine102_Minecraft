using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 10;
    public int currentHP;

    [Header("Drop Item")]
    // 몬스터가 죽으면 드롭할 아이템 (기본값: 석탄)
    public ItemType dropItem = ItemType.Coal;

    void Start()
    {
        currentHP = maxHP;

        // 태그가 설정 안 되어 있으면 자동으로 설정
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Enemy";
    }

    // 외부(플레이어)에서 호출하는 데미지 함수
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"몬스터 맞음! 남은 체력: {currentHP}");

        // 피격 효과 (빨간색 깜빡임 등)를 넣고 싶다면 여기에 작성

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("몬스터 처치!");

        // TODO: 나중에 아이템 드롭 기능(ItemDropper)이 있다면 여기서 호출
        // ItemDropper.Instance.Drop(dropItem, transform.position);

        Destroy(gameObject);
    }
}