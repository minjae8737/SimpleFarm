using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnvilPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Transform requiredItemParent;
    [SerializeField] private GameObject strengthUpgradeItemSlotPrefab;
    [SerializeField] private Button upgradeBtn;
    private List<GameObject> strengthUpgradeItemSlotList = new List<GameObject>();
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite[] btnSprites;

    public void SetPanel(PlayerStrength playerStrength)
    {
        PlayerStrengthUpgradeCondition condition = playerStrength.GetNextUpgradeCondition();
        
        int curStrength = playerStrength.Strength;
        bool isMaxStrength = playerStrength.IsMaxStrength();
        bool canUpgrade = playerStrength.CanUpgrade();
        
        curText.text = curStrength.ToString();
        nextText.text = isMaxStrength ? "Max" : (curStrength + 1).ToString();
        upgradeBtn.enabled = !isMaxStrength;
        upgradeBtn.GetComponent<Image>().sprite = canUpgrade ? btnSprites[1] : btnSprites[0];
        
        if (condition != null)
        {
            // 필요 아이템 추가
            for (int i = 0; i < condition.requiredItems.Length; i++)
            {
                // 아이템 데이터
                ItemType requiredItemtype = condition.requiredItems[i];
                ItemData itemData = GameManager.instance.GetItemData(requiredItemtype);
                long requiredItemCount = condition.requiredItemCounts[i];
                long curItemCount = GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());

                // 아이템 받기
                GameObject newItem = GetItem(i);
                string countText = string.Format("{0}/{1}", curItemCount, requiredItemCount);

                newItem.GetComponent<StrengthUpgradeItem>().SetDictionaryItem(itemData.icon, countText);
            }

            // 필요 골드 추가
            string convertGoldToText = GameManager.instance.uiManager.ConvertGoldToText(GameManager.instance.gold);
            string goldText = string.Format("{0}/{1}", convertGoldToText, condition.goldCost);

            GameObject newGoldItem = GetItem(condition.requiredItems.Length);

            newGoldItem.GetComponent<StrengthUpgradeItem>().SetDictionaryItem(goldIcon, goldText);
            strengthUpgradeItemSlotList.Add(newGoldItem);
        }
        
        
    }

    private GameObject GetItem(int index)
    {
        GameObject item = null;
        
        if (index >= strengthUpgradeItemSlotList.Count)
        {
            item = Instantiate(strengthUpgradeItemSlotPrefab, requiredItemParent);
            strengthUpgradeItemSlotList.Add(item);
        }
        else
        {
            item = strengthUpgradeItemSlotList[index];
        }

        return item;
    }

    public void OnClickUpgradeBtn()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.UIButtonClickEnabled);
        GameManager.instance.player.PlayerStrength.UpgradeStrength();
    }
}
