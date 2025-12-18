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

    // player 변수는 이제 필요 없으므로 삭제했습니다.

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
    }

    // [삭제됨] Start 함수에서 PlayerHarvester의 toolDamage를 건드리는 코드가 오류의 원인이었습니다.
    // 이제 데미지는 PlayerHarvester가 ItemData를 보고 알아서 계산하므로 이 코드는 필요 없습니다.

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭 시 조합 패널에 아이템 추가
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (!craftingPanel) return;

        craftingPanel.AddPlanned(blockType, 1);
    }
}