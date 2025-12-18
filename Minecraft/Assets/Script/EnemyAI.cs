using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 3f;
    public float detectRange = 10f; // 플레이어 감지 범위

    private Transform target;
    private Rigidbody rb;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 몬스터가 물리 충돌로 넘어지지 않게 회전 축 고정
        rb.freezeRotation = true;

        // 플레이어 찾기 (Tag가 "Player"인 오브젝트)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 1. 거리 계산
        float distance = Vector3.Distance(transform.position, target.position);

        // 2. 감지 범위 안에 들어왔는지 확인
        if (distance <= detectRange)
        {
            isChasing = true;
        }
        else
        {
            // 너무 멀어지면 추적 포기 (선택 사항)
            // isChasing = false; 
        }

        // 3. 추적 로직
        if (isChasing)
        {
            // 몬스터가 플레이어를 바라보게 함 (Y축 회전만)
            Vector3 lookPos = target.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            // 앞으로 이동
            Vector3 moveDir = transform.forward * moveSpeed;
            // Y축 속도(중력)는 유지하고 X, Z축만 이동
            rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
        }
    }
}