using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private Dictionary<string, long> items;

    public Dictionary<string, long> Items
    {
        get => items;
    }

    const string ItemKey = "Item_";
    
    public event Action<ItemData, long> OnItemAdded;
    public event Action<ItemData, long> OnItemRemoved;
    
    public void Init()
    {
        items =  new Dictionary<string, long>();
        
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            string key = ItemKey + type; // "Item_xxxx"
            items.Add(key, GameManager.instance.GetLongFromPlayerPrefs(key));
        }
    }

    public void AddItem(ItemData itemData, long quantity)
    {
        if (items.ContainsKey(ItemKey + itemData.type))
        {
            items[ItemKey + itemData.type] += quantity;
            if(items[ItemKey + itemData.type] > itemData.maxStackSize)
                items[ItemKey + itemData.type] = itemData.maxStackSize;
        }
        else
        {
            items.Add(ItemKey + itemData.type, quantity);
        }

        OnItemAdded?.Invoke(itemData, quantity);

    }

    public void RemoveItem(ItemData itemData, long quantity)
    {
        if (items.ContainsKey(ItemKey + itemData.type))
        {
            if (items[ItemKey + itemData.type] - quantity < 0)
                return;
            
            items[ItemKey + itemData.type] -= quantity;
            
            OnItemRemoved?.Invoke(itemData, quantity);
        }
    }

    public long GetItemQuantity(string itemType)
    {
        return items[ItemKey + itemType];
    }
}
