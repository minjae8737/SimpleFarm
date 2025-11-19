using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStrength
{
    private int strength;
    public int Strength => strength;
    private const int maxStrength = 10;
    public PlayerStrengthUpgradeCondition[] upgradeConditions;

    public event Action OnPlayerStrengthUpdated;

    public void UpgradeStrength()
    {
        if (IsMaxStrength() || !CanUpgrade()) return;

        PlayerStrengthUpgradeCondition condition = GetNextUpgradeCondition();
        
        strength++; // 힘 증가
        
        // 아이템 감소
        for (int i = 0; i < condition.requiredItemCounts.Length; i++)
        {
            ItemType requiredItemType = condition.requiredItems[i];
            long requiredQuantity = condition.requiredItemCounts[i];
            GameManager.instance.inventory.RemoveItem(requiredItemType,requiredQuantity);
        }
        
        // 골드 감소
        long requiredGoldCost = condition.goldCost;
        GameManager.instance.SetGold(-requiredGoldCost);
        OnPlayerStrengthUpdated?.Invoke();
    }

    public PlayerStrengthUpgradeCondition GetNextUpgradeCondition()
    {
        PlayerStrengthUpgradeCondition condition = IsMaxStrength() ? null : upgradeConditions[strength - 1];
        return condition;
    }

    public void SetStrength(int strength)
    {
        this.strength = strength;
    }

    public bool IsMaxStrength()
    {
        return strength >= maxStrength;
    }

    public bool CanUpgrade()
    {
        if (strength > upgradeConditions.Length) return false;
        
        bool hasItem = true;
        bool hasGold = true;
        
        PlayerStrengthUpgradeCondition condition = upgradeConditions[strength - 1];

        // 아이템 체크
        for (int i = 0; i < condition.requiredItemCounts.Length; i++)
        {
            ItemType requiredItemType = condition.requiredItems[i];
            long requiredQuantity = condition.requiredItemCounts[i];
            
            hasItem = GameManager.instance.inventory.HasItem(requiredItemType, requiredQuantity);
            if (!hasItem) break;
        }

        // 골드 체크
        hasGold = GameManager.instance.CheckGold(condition.goldCost);
        
        return hasItem && hasGold;
    }
    
}
