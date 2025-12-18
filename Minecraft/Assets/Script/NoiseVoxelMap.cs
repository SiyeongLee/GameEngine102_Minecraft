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
    public GameObject blockPrefabStone;
    public GameObject blockPrefabCoal;
    public GameObject blockPrefabIron;
    public GameObject blockPrefabWood;
    public GameObject blockPrefabLeaf;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;      // [추가됨] 몬스터 프리팹 연결용
    [Range(0, 1)] public float enemyProbability = 0.02f; // [추가됨] 몬스터 생성 확률 (2%)

    [Header("Map Settings")]
    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    public int waterLevel = 4;

    [SerializeField] float noiseScale = 20f;

    [Header("Generation Rates")]
    [Range(0, 1)] public float treeProbability = 0.05f;

    private float offsetX;
    private float offsetZ;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        offsetX = Random.Range(-9999f, 9999f);
        offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);
                int h = Mathf.FloorToInt(noise * maxHeight);

                if (h < 1) h = 1;

                for (int y = 0; y <= h; y++)
                {
                    ItemType typeToPlace = ItemType.Dirt;
                    GameObject prefabToUse = blockPrefabDirt;

                    // (A) 지표면
                    if (y == h)
                    {
                        if (y >= waterLevel) // 물 위 (땅)
                        {
                            typeToPlace = ItemType.Grass;
                            prefabToUse = blockPrefabGrass;

                            // 1. 나무 심기 시도
                            if (Random.value < treeProbability)
                            {
                                GenerateTree(x, y + 1, z);
                            }
                            // 2. [추가됨] 몬스터 소환 시도 (나무가 없는 곳에)
                            else if (enemyPrefab != null && Random.value < enemyProbability)
                            {
                                // y + 1.5f 높이에 생성하여 땅에 끼이는 것 방지
                                Instantiate(enemyPrefab, new Vector3(x, y + 1.5f, z), Quaternion.identity, transform);
                            }
                        }
                        else // 물 밑
                        {
                            typeToPlace = ItemType.Dirt;
                            prefabToUse = blockPrefabDirt;
                        }
                    }
                    // (B) 지하
                    else
                    {
                        if (h - y > 3)
                        {
                            float val = Random.value;
                            if (val < 0.05f) { typeToPlace = ItemType.Iron; prefabToUse = blockPrefabIron; }
                            else if (val < 0.15f) { typeToPlace = ItemType.Coal; prefabToUse = blockPrefabCoal; }
                            else { typeToPlace = ItemType.Stone; prefabToUse = blockPrefabStone; }
                        }
                        else
                        {
                            typeToPlace = ItemType.Dirt;
                            prefabToUse = blockPrefabDirt;
                        }
                    }

                    CreateBlock(x, y, z, prefabToUse, typeToPlace);
                }

                // 물 채우기
                for (int y = h + 1; y < waterLevel; y++)
                {
                    CreateBlock(x, y, z, blockPrefabWater, ItemType.Water);
                }
            }
        }
    }

    void GenerateTree(int x, int y, int z)
    {
        int height = Random.Range(3, 6);
        for (int i = 0; i < height; i++)
        {
            CreateBlock(x, y + i, z, blockPrefabWood, ItemType.Wood);
        }

        int topY = y + height;
        CreateBlock(x, topY, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x + 1, topY - 1, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x - 1, topY - 1, z, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x, topY - 1, z + 1, blockPrefabLeaf, ItemType.Leaf);
        CreateBlock(x, topY - 1, z - 1, blockPrefabLeaf, ItemType.Leaf);
    }

    void CreateBlock(int x, int y, int z, GameObject prefab, ItemType type)
    {
        if (prefab == null) return;

        // 맵 범위 체크 해제됨 (무한 확장 지원)
        // if (x < 0 || x >= width || z < 0 || z >= depth) return;

        var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"{type}_{x}_{y}_{z}";

        var b = go.GetComponent<Block>();
        if (b == null) b = go.AddComponent<Block>();

        b.type = type;

        if (type == ItemType.Stone || type == ItemType.Iron) b.maxHP = 5;
        else if (type == ItemType.Wood) b.maxHP = 4;
        else b.maxHP = 3;

        // 블록 약점 도구 자동 설정 (편의 기능 추가)
        if (type == ItemType.Stone || type == ItemType.Iron || type == ItemType.Coal)
            b.effectiveTool = ToolType.Pickaxe;
        else if (type == ItemType.Wood)
            b.effectiveTool = ToolType.Axe;
    }

    public void PlaceTile(Vector3Int pos, ItemType type)
    {
        GameObject prefab = blockPrefabDirt;
        switch (type)
        {
            case ItemType.Dirt: prefab = blockPrefabDirt; break;
            case ItemType.Grass: prefab = blockPrefabGrass; break;
            case ItemType.Water: prefab = blockPrefabWater; break;
            case ItemType.Stone: prefab = blockPrefabStone; break;
            case ItemType.Wood: prefab = blockPrefabWood; break;
            case ItemType.Leaf: prefab = blockPrefabLeaf; break;
            case ItemType.Coal: prefab = blockPrefabCoal; break;
            case ItemType.Iron: prefab = blockPrefabIron; break;
            default: prefab = blockPrefabDirt; break;
        }
        CreateBlock(pos.x, pos.y, pos.z, prefab, type);
    }
}