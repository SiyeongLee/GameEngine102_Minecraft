using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{
    [Header("Basic Block Prefabs")]
    public GameObject blockPrefabDirt;
    public GameObject blockPrefabGrass;
    public GameObject blockPrefabWater;

    [Header("Resource Block Prefabs")]
    public GameObject blockPrefabStone; // 인스펙터 연결 필수
    public GameObject blockPrefabCoal;  // 인스펙터 연결 필수
    public GameObject blockPrefabIron;  // 인스펙터 연결 필수
    public GameObject blockPrefabWood;  // 인스펙터 연결 필수
    public GameObject blockPrefabLeaf;  // 인스펙터 연결 필수

    [Header("Map Settings")]
    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    public int waterLevel = 4;

    [SerializeField] float noiseScale = 20f;

    [Header("Generation Rates")]
    [Range(0, 1)] public float treeProbability = 0.05f; // 나무 생성 확률 5%

    private float offsetX;
    private float offsetZ;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // 매번 다른 지형을 만들기 위한 랜덤 오프셋
        offsetX = Random.Range(-9999f, 9999f);
        offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 1. 펄린 노이즈로 높이 계산
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);
                int h = Mathf.FloorToInt(noise * maxHeight);

                if (h < 1) h = 1;

                // 2. 바닥부터 높이 h까지 블록 쌓기
                for (int y = 0; y <= h; y++)
                {
                    ItemType typeToPlace = ItemType.Dirt;
                    GameObject prefabToUse = blockPrefabDirt;

                    // (A) 맨 위층 (지표면)
                    if (y == h)
                    {
                        if (y >= waterLevel) // 물 위라면 잔디
                        {
                            typeToPlace = ItemType.Grass;
                            prefabToUse = blockPrefabGrass;

                            // 지표면 위에 나무 심기 시도
                            if (Random.value < treeProbability)
                            {
                                GenerateTree(x, y + 1, z);
                            }
                        }
                        else // 물 밑이라면 흙(또는 모래)
                        {
                            typeToPlace = ItemType.Dirt;
                            prefabToUse = blockPrefabDirt;
                        }
                    }
                    // (B) 지하 (광물 생성)
                    else
                    {
                        // 지표면보다 3칸 이상 깊은 곳부터 광물 등장
                        if (h - y > 3)
                        {
                            float val = Random.value;
                            if (val < 0.05f) // 5% 확률로 철
                            {
                                typeToPlace = ItemType.Iron;
                                prefabToUse = blockPrefabIron;
                            }
                            else if (val < 0.15f) // 10% 확률로 석탄
                            {
                                typeToPlace = ItemType.Coal;
                                prefabToUse = blockPrefabCoal;
                            }
                            else // 나머지는 돌
                            {
                                typeToPlace = ItemType.Stone;
                                prefabToUse = blockPrefabStone;
                            }
                        }
                        else // 지표면 바로 아래는 그냥 흙
                        {
                            typeToPlace = ItemType.Dirt;
                            prefabToUse = blockPrefabDirt;
                        }
                    }

                    CreateBlock(x, y, z, prefabToUse, typeToPlace);
                }

                // 3. 물 채우기
                for (int y = h + 1; y < waterLevel; y++)
                {
                    CreateBlock(x, y, z, blockPrefabWater, ItemType.Water);
                }
            }
        }
    }

    void GenerateTree(int x, int y, int z)
    {
        // 나무 기둥 (높이 3~5칸 랜덤)
        int height = Random.Range(3, 6);
        for (int i = 0; i < height; i++)
        {
            CreateBlock(x, y + i, z, blockPrefabWood, ItemType.Wood);
        }

        // 나뭇잎 (기둥 꼭대기 주변)
        int topY = y + height;
        CreateBlock(x, topY, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x + 1, topY - 1, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x - 1, topY - 1, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x, topY - 1, z + 1, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x, topY - 1, z - 1, blockPrefabLeaf, ItemType.Leaf);
    }

    // 블록 생성 헬퍼 함수
    void CreateBlock(int x, int y, int z, GameObject prefab, ItemType type)
    {
        if (prefab == null) return;
        // 맵 범위 체크
        if (x < 0 || x >= width || z < 0 || z >= depth) return;

        var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"{type}_{x}_{y}_{z}";

        var b = go.GetComponent<Block>();
        if (b == null) b = go.AddComponent<Block>();

        b.type = type;

        // 블록 타입별 체력 설정 (옵션)
        if (type == ItemType.Stone || type == ItemType.Iron) b.maxHP = 5;
        else if (type == ItemType.Wood) b.maxHP = 4;
        else b.maxHP = 3;
    }

    // PlayerHarvester 등 외부에서 블록 설치 시 호출하는 함수
    public void PlaceTile(Vector3Int pos, ItemType type)
    {
        GameObject prefab = blockPrefabDirt;
        switch (type)
        {
            case ItemType.Grass: prefab = blockPrefabGrass; break;
            case ItemType.Water: prefab = blockPrefabWater; break;
            case ItemType.Stone: prefab = blockPrefabStone; break;
            case ItemType.Wood: prefab = blockPrefabWood; break;
                // 필요한 경우 다른 타입도 추가
        }
        CreateBlock(pos.x, pos.y, pos.z, prefab, type);
    }
}