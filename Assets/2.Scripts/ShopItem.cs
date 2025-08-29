using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public Image icon; // 아이템 이미지
    public Text quantityText; // 소지 개수
    public Text priceText; // 개별 판매 금액
    
    // 아이템 세팅
    public void Init(ItemData itemData)
    {
        itemName = itemData.itemName;
        icon.sprite = itemData.icon;
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.itemName);
        RefreshQuantity(itemQuantity);
        priceText.text = itemData.price.ToString();
    }

    public void RefreshQuantity(long quantity)
    {
        quantityText.text = "x" + quantity;
    }
    
}
