using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float rayDistance = 5f;          // 채집 가능 거리
    public LayerMask hitMask = ~0;          // 충돌 체크할 레이어
    public int toolDamage = 1;              // 도구 데미지 (기본값)
    public float hitCooldown = 0.15f;       // 공격 속도

    private float _nextHitTime;
    private Camera _cam;

    [Header("References")]
    public Inventory inventory;
    public InventoryUI inventoryUI;
    public GameObject selectedBlock;        // 설치 미리보기 블록

    void Awake()
    {
        _cam = Camera.main;
        if (inventory == null) inventory = gameObject.AddComponent<Inventory>();

        // InventoryUI가 씬에 없으면 찾아서 연결
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        // UI가 없거나 인벤토리가 닫혀있을 때만 동작
        if (inventoryUI == null) return;

        // 설치 모드인지 확인 (selectedIndex가 0 이상이면 설치 모드)
        if (inventoryUI.selectedIndex < 0)
        {
            // [공격 및 채집 모드]
            selectedBlock.transform.localScale = Vector3.zero; // 미리보기 숨김

            if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
            {
                _nextHitTime = Time.time + hitCooldown;
                DoAttack(); // 공격 함수 호출
            }
        }
        else
        {
            // [블록 설치 모드]
            // 현재 선택된 아이템 타입 가져오기
            ItemType selectedType = inventoryUI.GetInventorySlot();
            HandleBlockPlacing(selectedType);
        }
    }

    // 공격 및 채집 처리 함수 (ItemData 없이 단순 동작)
    void DoAttack()
    {
        // 화면 정중앙으로 레이 발사
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            // A. 몬스터 타격 확인 (Enemy 스크립트가 있을 경우)
            var enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(toolDamage); // 설정된 데미지만큼 입힘

                // 넉백(밀려남) 효과
                Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(transform.forward * 5f, ForceMode.Impulse);
                }
                return; // 몬스터를 때렸으면 블록은 안 때림
            }

            // B. 블록 채집 확인
            var block = hit.collider.GetComponent<Block>();
            if (block != null)
            {
                // 블록의 Hit 함수 호출 (단순 데미지 전달)
                block.Hit(toolDamage, inventory);
            }
        }
    }

    // 블록 설치 처리 함수
    void HandleBlockPlacing(ItemType typeToPlace)
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // 1. 미리보기 위치 계산
        if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            Vector3Int placePos = AdjacentCellOnHitFace(hit);

            selectedBlock.transform.localScale = Vector3.one;
            selectedBlock.transform.position = placePos;
            selectedBlock.transform.rotation = Quaternion.identity;

            // 2. 좌클릭 시 실제 설치
            if (Input.GetMouseButtonDown(0))
            {
                // 인벤토리에서 아이템 1개 소모 시도
                if (inventory.Consume(typeToPlace, 1))
                {
                    // 맵 생성 스크립트를 찾아 블록 설치 요청
                    var map = FindObjectOfType<NoiseVoxelMap>();
                    if (map != null)
                    {
                        map.PlaceTile(placePos, typeToPlace);
                    }
                }
            }
        }
        else
        {
            selectedBlock.transform.localScale = Vector3.zero; // 허공을 보고 있으면 미리보기 숨김
        }
    }

    // 레이가 맞은 면의 인접한 좌표(블록 설치 위치) 계산
    static Vector3Int AdjacentCellOnHitFace(in RaycastHit hit)
    {
        Vector3 baseCenter = hit.collider.transform.position;
        Vector3 adjCenter = baseCenter + hit.normal;
        return Vector3Int.RoundToInt(adjCenter);
    }
}