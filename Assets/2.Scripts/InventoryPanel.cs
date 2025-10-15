using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{

    [Header("")]
    public GameObject inventoryItemPrefab;
    private List<GameObject> inventoryItems;
    
    [Header("UI")]
    public Transform content;
    
    public void Init()
    {
        inventoryItems = new  List<GameObject>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if(type == ItemType.None) continue;
            
            GameObject newInventoryItem = Instantiate(inventoryItemPrefab, content);
            inventoryItems.Add(newInventoryItem);
            InventoryItem inventoryItem = newInventoryItem.GetComponent<InventoryItem>();
            inventoryItem.Init(GameManager.instance.itemDatas[(int)type]);
        }
    }
    
    public void RefreshInventoryPanel(ItemData itemData)
    {
        GameObject findInventoryItem = inventoryItems.Find(item => item.GetComponent<InventoryItem>().itemName == itemData.itemName);
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());
        findInventoryItem?.GetComponent<InventoryItem>().RefreshQuantity(itemQuantity);
    }
}
