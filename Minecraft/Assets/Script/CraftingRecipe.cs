using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "조합법 생성")]
public class CraftingRecipe : ScriptableObject
{
    [Serializable]
    public struct Ingredient
    {
        public ItemType type;
        public int count;
    } // 재료

    [Serializable]
    public struct Product
    {
        public ItemType type;
        public int count;
    } // 결과물

    public string displayName; // 레시피 이름 (예: 돌 곡괭이)
    public List<Ingredient> inputs = new();  // 필요한 재료 목록
    public List<Product> outputs = new();    // 나오는 결과물 목록
}