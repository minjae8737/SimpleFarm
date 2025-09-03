using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemSellPanel : MonoBehaviour
{
    UIManager uiManager;
    ItemData itemData;
    
    [Header("Sell Info")] 
    public Image productIcon;
    public Text productQuantity;
    public Text productPriceText;
    
    [Header("Slider")] 
    public Slider slider;
    
    [Header("Sell Btn")]
    public Text totalPriceText;

    public event Action<ItemData, long> OnItemSell;
    public event Action OffItemInfoPanel;

    private void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f),0.2f, 1, 1f);
    }

    public void Init(ItemData itemData,UIManager uiManager)
    {
        this.uiManager = uiManager;
        this.itemData = itemData;
        Refresh();
    }

    void Refresh()
    {
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());
        
        productIcon.sprite = itemData.icon;
        productQuantity.text = itemQuantity.ToString();
        productPriceText.text = itemData.price.ToString();

        slider.maxValue = itemQuantity;
        slider.value = itemQuantity;

        totalPriceText.text = uiManager.ConvertGoldToText(itemQuantity * itemData.price);
    }

    public void OnChangedValue()
    {
        productQuantity.text = slider.value.ToString();
        totalPriceText.text = uiManager.ConvertGoldToText((long)slider.value * itemData.price);
    }

    public void OnClickSellButton()
    {
        if (slider.value <= 0) 
            return;
        
        OnItemSell?.Invoke(itemData, (long)slider.value);
        Refresh();
    }
    
    public void OnClickExitButton()
    {
        OffItemInfoPanel?.Invoke();
        gameObject.SetActive(false);
    }
    
}
