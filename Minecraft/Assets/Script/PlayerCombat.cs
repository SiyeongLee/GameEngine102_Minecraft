using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectilePrefab; // Projectile 프리팹 연결 (인스펙터)
    public Transform firePoint;         // 발사 위치 (Main Camera 연결 권장)
    public float fireForce = 20f;       // 발사 속도

    private Inventory inventory;

    void Start()
    {
        inventory = GetComponent<Inventory>();

        // 발사 위치가 따로 설정 안 되어 있으면 메인 카메라 사용
        if (firePoint == null)
        {
            firePoint = Camera.main.transform;
        }
    }

    void Update()
    {
        // 마우스 오른쪽 클릭 시 발사 시도
        if (Input.GetMouseButtonDown(1))
        {
            // 인벤토리에 'Weapon' 아이템이 있는지 확인
            if (inventory != null && inventory.items.ContainsKey(BlockType.Weapon))
            {
                Shoot();
            }
            else
            {
                Debug.Log("무기가 없습니다! (나무 5개를 모아 숫자키 2번을 눌러 제작하세요)");
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        // 1. 총알 생성 (카메라 약간 앞쪽에서)
        Vector3 spawnPos = firePoint.position + firePoint.forward * 1.5f;
        GameObject bullet = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        // 2. 물리 힘(Force) 가하기
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody>();
        }

        rb.velocity = firePoint.forward * fireForce;

        // 3. 3초 뒤 자동 삭제 (메모리 관리)
        Destroy(bullet, 3f);
    }
}