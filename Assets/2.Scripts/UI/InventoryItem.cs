using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    ItemData itemData;
    public string itemName;
    public Image icon; // 아이템 이미지
    public TextMeshProUGUI quantityText; // 소지 개수
    
    public event Action<ItemData> OnClickInventoryItem; 
    
    // 아이템 세팅
    public void Init(ItemData itemData)
    {
        this.itemData = itemData;
        itemName = itemData.itemName;
        icon.sprite = itemData.icon;
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());
        RefreshQuantity(itemQuantity);
        OnClickInventoryItem += GameManager.instance.uiManager.OnItemInfoPanel;

    }
    
    public void RefreshQuantity(long quantity)
    {
        quantityText.text = "x" + quantity;
    }

    public void OnClickItem()
    {
        OnClickInventoryItem?.Invoke(itemData);
    }

}
