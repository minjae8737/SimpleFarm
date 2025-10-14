using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("# Item")] 
    public Inventory inventory;
    public ItemData[] itemDatas;

    [Header("# Manager")] 
    public QuestManager questManager;
    public IslandManager islandManager;
    public ObjectPoolManager objectPoolManager;
    public UIManager uiManager;
    
    public Timer timer;
    
    const string GoldKey = "Gold";
    public long gold;
    long maxGold = 9999999999; // 9,999,999,999
    public bool canPickUp = true;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Init();

        questManager.Init();
        islandManager.Init();
        objectPoolManager.Init();
        uiManager.Init();
    }

    private void OnApplicationQuit()
    {
        SaveGold();
    }

    void Init()
    {
        // 골드 초기화
        gold = GetLongFromPlayerPrefs(GoldKey);

        // 인벤토리 초기화
        inventory = new Inventory();
        inventory.Init();

        uiManager.itemSellPanel.OnItemSell += SellItem;
    }

    public long GetLongFromPlayerPrefs(string key, long defaultValue = 0L)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        string longStr = PlayerPrefs.GetString(key);
        if (long.TryParse(longStr, out long result))
            return result;

        return defaultValue;
    }
    
    public int GetIntFromPlayerPrefs(string key, int defaultValue = 0)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        string longStr = PlayerPrefs.GetString(key);
        if (int.TryParse(longStr, out int result))
            return result;

        return defaultValue;
    }
    
    public void SaveLongToPlayerPrefs(string key, long value)
    {
        PlayerPrefs.SetString(key, value.ToString());
        PlayerPrefs.Save();
    }
    
    public void SaveIntToPlayerPrefs(string key, int value)
    {
        PlayerPrefs.SetString(key, value.ToString());
        PlayerPrefs.Save();
    }

    #region Gold

    public bool CheckGold(long price)
    {
        return price <= gold;
    }

    public void SetGold(long price)
    {
        gold += price;
        if (gold > maxGold) gold = maxGold;
        uiManager.RefreshGoldText();
    }

    void SaveGold()
    {
        SaveLongToPlayerPrefs(GoldKey, gold);
    }

    #endregion

    public void RestPlayer()
    {
        player.RecoverHP();
        uiManager.RecorvePlayerHpEffect();
    }

    #region Item

    public GameObject GetDropItem(ItemType type)
    {
        return objectPoolManager.Get(type);
    }

    public void PickUpItem(ItemData itemData)
    {
        inventory.AddItem(itemData, 1);
    }

    public ItemData GetItemData(ItemType type)
    {
        return itemDatas[(int)type];
    }

    void SellItem(ItemData itemData, long quantity)
    {
        inventory.RemoveItem(itemData, quantity);
        SetGold(itemData.price * quantity);
        uiManager.RefreshGoldText();
    }

    public void PickUpAllItems()
    {
        if (!canPickUp) return;
        
        canPickUp = false;
        TimerHandler timerHandler = timer.StartTimer(30f, PickUpItemsCooldownEnd);
        uiManager.OnClickMagnetBtn(timerHandler);
        objectPoolManager.PickUpAllItems();
    }

    public void PickUpItemsCooldownEnd()
    {
        canPickUp = true;
    }

    #endregion
}