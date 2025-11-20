using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum BlockType { Dirt, Grass, Water }


public class Block : MonoBehaviour
{
    [Header("Block Stat")]
   
    public BlockType type = BlockType.Dirt;

   
    public int maxHP = 3;

    
    [HideInInspector] public int hp;

   
    public int dropCount = 1;

  
    public bool mineable = true;

    void Awake()
    {
        
        hp = maxHP;

        
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

       
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
        {
            gameObject.tag = "Block";
        }
    }

 
    public void Hit(int damage, Inventory inven)
    {
        if (!mineable) return;

        hp -= damage;

        Debug.Log("1");
        if (hp <= 0)
        {
            Debug.Log("2");

            if (inven != null && dropCount > 0)
            {
                Debug.Log("dkdkk3d");
                inven.Add(type, dropCount);
            }
            Destroy(gameObject);


        }
    }
}

