using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Minecraft/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;       // 아이템 이름 (예: Stone Pickaxe)
    public ItemType itemType;     // Block.cs에 있는 Enum과 일치시켜야 함
    public Sprite icon;           // 인벤토리에 표시될 이미지

    [Header("도구 성능")]
    public ToolType toolType = ToolType.None; // 이 아이템은 무슨 도구인가?
    public int baseDamage = 1;                // 기본 데미지 (맨손급)
    public int bonusDamage = 5;               // 약점 블록을 캘 때의 데미지
}