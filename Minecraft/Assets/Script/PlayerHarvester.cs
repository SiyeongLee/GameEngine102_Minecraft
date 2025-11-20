using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    public float rayDistance = 5f;
    public LayerMask hitMask = ~0;
    public int toolDamage = 1;
    public float hitCooldown = 0.15f;

    private float _nextHitTime;
    private Camera _cam;

    public Inventory inventory;
    InventoryUI InvenUI;

    void Awake()
    {
        _cam = Camera.main;
        if (inventory == null) inventory = gameObject.AddComponent<Inventory>();
        InvenUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        
        if (InvenUI.selectedIndex < 0)
        {
            if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
            {
                _nextHitTime = Time.time + 0.15f; 

                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    var block = hit.collider.GetComponent<Block>();

                    if (block != null)
                    {
                       
                        block.Hit(toolDamage, inventory);
                    }
                }
            }
        }
        else 
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                
                    Vector3Int placePos = AdjacentCellOnHitFace(hit);

                   
                    BlockType selected = InvenUI.GetInventorySlot();

                   
                    if (inventory.Consume(selected, 1))
                    {
              
                        FindObjectOfType<NoiseVoxelMap>().PlaceTile(placePos, selected);
                    }
                }
            }
        }
    }

    
    public Vector3Int AdjacentCellOnHitFace(RaycastHit hit)
    {
       
        Vector3 baseCenter = hit.collider.transform.position;
        Vector3 adjCenter = baseCenter + hit.normal;

        return Vector3Int.RoundToInt(adjCenter);
    }
}