using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotItemPrefab : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public TextMeshProUGUI itemText;
    public ItemType blockType;
    public CraftingPanel craftingPanel;
    private GameObject player;

    public void ItemSetting(Sprite itemSprite, string txt, ItemType type)
    {
        itemImage.sprite = itemSprite;
        itemText.text = txt;
        blockType = type;
    }

    void Awake()
    {
        if (!craftingPanel)
            craftingPanel = FindObjectOfType<CraftingPanel>(true);
        player = GameObject.FindWithTag("Player");


    }

    private void Start()
    {
        if (blockType == ItemType.Axe)
        {
            player.GetComponent<PlayerHarvester>().toolDamage = 2;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (!craftingPanel) return;

        craftingPanel.AddPlanned(blockType, 1);
    }
}