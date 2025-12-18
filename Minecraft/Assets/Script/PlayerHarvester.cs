using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float rayDistance = 5f;          // 사거리
    public LayerMask hitMask = ~0;          // 모든 레이어 충돌 허용
    public float hitCooldown = 0.2f;        // 공격 속도

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
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        // UI가 없거나 인벤토리가 닫혀있을 때만 동작
        if (inventoryUI == null) return;

        // 1. 현재 선택된 아이템 확인
        ItemType selectedType = inventoryUI.GetInventorySlot();

        // 2. 도구 데이터 가져오기 (만약 ItemDataManager를 안 만들었다면 null)
        ItemData toolData = null;
        if (ItemDataManager.Instance != null)
        {
            toolData = ItemDataManager.Instance.GetItemData(selectedType);
        }

        // 3. 모드 결정 (설치 모드 vs 도구/맨손 모드)
        // selectedIndex가 0보다 작으면(선택 안함) 공격/채집 모드
        // 만약 도구(곡괭이 등)라면 설치하면 안 되므로 공격 모드로 처리
        bool isTool = (toolData != null && toolData.toolType != ToolType.None);

        if (inventoryUI.selectedIndex < 0 || isTool)
        {
            // [공격 및 채집 모드]
            selectedBlock.transform.localScale = Vector3.zero; // 미리보기 숨김

            if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
            {
                _nextHitTime = Time.time + hitCooldown;
                DoAttack(toolData); // 공격 함수 호출
            }
        }
        else
        {
            // [블록 설치 모드]
            HandleBlockPlacing(selectedType);
        }
    }

    // 공격 및 채집 처리 함수
    void DoAttack(ItemData toolData)
    {
        // 화면 정중앙으로 레이 발사
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            // A. 몬스터 타격 확인
            var enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 도구 데미지 적용 (없으면 기본 1)
                int damage = (toolData != null) ? toolData.bonusDamage : 1;

                enemy.TakeDamage(damage);

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
                int damage = 1; // 기본 데미지

                // 도구 효율 계산
                if (toolData != null)
                {
                    // 블록의 약점 도구와 내 도구가 일치하면 보너스 데미지
                    if (block.effectiveTool == toolData.toolType && block.effectiveTool != ToolType.None)
                    {
                        damage = toolData.bonusDamage;
                    }
                    else
                    {
                        damage = toolData.baseDamage;
                    }
                }

                block.Hit(damage, inventory);
            }
        }
    }

    // 블록 설치 처리 함수 (기존 코드 유지)
    void HandleBlockPlacing(ItemType typeToPlace)
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            Vector3Int placePos = AdjacentCellOnHitFace(hit);

            // 미리보기 표시
            selectedBlock.transform.localScale = Vector3.one;
            selectedBlock.transform.position = placePos;
            selectedBlock.transform.rotation = Quaternion.identity;

            // 좌클릭 시 실제 설치
            if (Input.GetMouseButtonDown(0))
            {
                // 재료 소모 시도
                if (inventory.Consume(typeToPlace, 1))
                {
                    FindObjectOfType<NoiseVoxelMap>().PlaceTile(placePos, typeToPlace);
                }
            }
        }
        else
        {
            selectedBlock.transform.localScale = Vector3.zero;
        }
    }

    static Vector3Int AdjacentCellOnHitFace(in RaycastHit hit)
    {
        Vector3 baseCenter = hit.collider.transform.position;
        Vector3 adjCenter = baseCenter + hit.normal;
        return Vector3Int.RoundToInt(adjCenter);
    }
}