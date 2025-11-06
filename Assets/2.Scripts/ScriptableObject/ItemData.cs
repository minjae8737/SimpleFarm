using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None = -1,
    Tree,
    Rock,
    Wheat,
    Beet,
    Cabbage,
    Carrot,
    Cauliflower,
    Kale,
    Parsnip,
    Potato,
    Pumpkin,
    Radish,
    Sunflower,
}

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObject/Item")]
public class ItemData : ScriptableObject
{
    [Header("# Info")] 
    public ItemType type;
    public string itemName;
    public string description;
    public int maxStackSize = 99999;

    [Header("# Shop")] public long price;
    public Sprite icon;
}