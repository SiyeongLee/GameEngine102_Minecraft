using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingPanel : MonoBehaviour
{
    public Inventory inventory;
    public List<CraftingRecipe> recipeList; // 에디터에서 레시피들을 여기에 등록해야 함
    public GameObject root;
    public TMP_Text plannedText;
    public Button craftButton;
    public Button clearButton;
    public TMP_Text hintText;

    // 현재 조합창에 올려둔 재료들 (아이템타입, 개수)
    readonly Dictionary<ItemType, int> planned = new();

    bool isOpen;

    void Start()
    {
        SetOpen(false);
        if (craftButton) craftButton.onClick.AddListener(DoCraft);
        if (clearButton) clearButton.onClick.AddListener(ClearPlanned);
        RefreshPlannedUI();
    }

    void Update()
    {
        // E키로 조합창 열기/닫기
        if (Input.GetKeyDown(KeyCode.E))
            SetOpen(!isOpen);
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (root)
            root.SetActive(open);

        if (!open)
            ClearPlanned(); // 닫으면 올려둔 재료 초기화
    }

    // 인벤토리에서 아이템을 우클릭하면 호출됨
    public void AddPlanned(ItemType type, int count = 1)
    {
        if (!planned.ContainsKey(type))
            planned[type] = 0;
        planned[type] += count;

        RefreshPlannedUI();
        SetHint($"{type} x{count} 추가 완료");
    }

    public void ClearPlanned()
    {
        planned.Clear();
        RefreshPlannedUI();
        SetHint("초기화 완료");
    }

    void RefreshPlannedUI()
    {
        if (!plannedText)
            return;

        if (planned.Count == 0)
        {
            plannedText.text = "우클릭으로 재료를 추가하세요.";
            return;
        }

        var sb = new StringBuilder();

        foreach (var item in planned)
            sb.AppendLine($"{item.Key} x{item.Value}");

        plannedText.text = sb.ToString();
    }

    void SetHint(string msg)
    {
        if (hintText)
            hintText.text = msg;
    }

    void DoCraft()
    {
        if (planned.Count == 0)
        {
            SetHint("재료가 부족합니다.");
            return;
        }

        // 1. 실제 인벤토리에 재료가 충분한지 확인
        foreach (var plannedItem in planned)
        {
            if (inventory.GetCount(plannedItem.Key) < plannedItem.Value)
            {
                SetHint($"{plannedItem.Key} 가 부족합니다.");
                return;
            }
        }

        // 2. 맞는 레시피 찾기
        var matchedRecipe = FindMatch(planned);
        if (matchedRecipe == null)
        {
            SetHint("알맞는 레시피가 없습니다.");
            return;
        }

        // 3. 재료 소모
        foreach (var itemforConsume in planned)
            inventory.Consume(itemforConsume.Key, itemforConsume.Value);

        // 4. 결과물 지급
        foreach (var p in matchedRecipe.outputs)
            inventory.Add(p.type, p.count);

        ClearPlanned();

        SetHint($"조합 완료 : {matchedRecipe.displayName}");
    }

    // 내가 올린 재료(planned)와 정확히 일치하는 레시피를 찾음
    CraftingRecipe FindMatch(Dictionary<ItemType, int> planned)
    {
        foreach (var recipe in recipeList)
        {
            // 재료 종류 개수가 다르면 패스
            // (주의: 이 로직은 정확히 레시피 재료만 올려야 성공함. 더 많이 올리면 실패 처리됨)
            if (recipe.inputs.Count != planned.Count) continue;

            bool ok = true;
            foreach (var ing in recipe.inputs)
            {
                // 레시피의 재료가 내 planned에 없거나, 개수가 다르면 실패
                if (!planned.TryGetValue(ing.type, out int have) || have != ing.count)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                return recipe;
        }
        return null;
    }
}