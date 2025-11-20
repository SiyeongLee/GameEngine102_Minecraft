using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{

    public GameObject blockPrefabDirt;
    public GameObject blockPrefabGrass;
    public GameObject blockPrefabWater;


    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    public int waterLevel = 4;

    [SerializeField] float noiseScale = 20f;


    private float offsetX;
    private float offsetZ;

    void Start()
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
                    if (y == h)
                    {

                        PlaceGrass(x, y, z);
                    }
                    else
                    {
                        PlaceDirt(x, y, z);
                    }
                }

                for (int y = h + 1; y < waterLevel; y++)
                {
                    PlaceWater(x, y, z);
                }
            }
        }
    }



    private void PlaceWater(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWater, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Water_{x}_{y}_{z}";
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = BlockType.Water;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

    private void PlaceDirt(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabDirt, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Dirt_{x}_{y}_{z}";
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = BlockType.Dirt;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

    private void PlaceGrass(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabGrass, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Grass_{x}_{y}_{z}";
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = BlockType.Grass;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }
    public void PlaceTile(Vector3Int pos, BlockType type)
    {
        switch (type)
        {
            case BlockType.Dirt:
                PlaceDirt(pos.x, pos.y, pos.z);
                break;
            case BlockType.Grass:
                PlaceGrass(pos.x, pos.y, pos.z);
                break;
            case BlockType.Water:
                PlaceWater(pos.x, pos.y, pos.z);
                break;
        }
    }
}