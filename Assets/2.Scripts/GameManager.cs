using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Dictionary<string, long> inventory;

    [Header("# Manager")]
    public QuestManager questManager;
    public IslandManager islandManager;
    public ObjectPoolManager objectPoolManager;
    public UIManager uiManager;

    const string GoldKey = "Gold";
    const string ItemKey = "Item_";
    public long gold;
    long maxGold = 9999999999; // 9,999,999,999

    public event Action<string> pickedItem;

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

    void Init()
    {
        // 골드 초기화
        gold = GetLongFromPlayerPrefs(GoldKey);

        // 인벤토리 초기화
        inventory = new Dictionary<string, long>();

        foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
        {
            string key = ItemKey + type.ToString(); // "Item_xxxx"
            inventory.Add(type.ToString(), GetLongFromPlayerPrefs(key));
        }

    }

    private long GetLongFromPlayerPrefs(string key, long defaultValue = 0L)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        string longStr = PlayerPrefs.GetString(key);
        if (long.TryParse(longStr, out long result))
            return result;

        return defaultValue;
    }

    bool CheckGold(long price)
    {
        return price <= gold;
    }

    void SetGold(long price)
    {
        gold += price;
        if (gold > maxGold) gold = maxGold;
    }

    void SaveGold()
    {
        PlayerPrefs.SetString(GoldKey, gold.ToString());
    }

    void RestPlayer()
    {
        player.RecoverHP();
    }

    public GameObject GetDropItem(ObjectType type)
    {
        return objectPoolManager.Get(type);
    }

    public void PickUpItem(ObjectType type)
    {
        string itemTypeStr = type.ToString();
        pickedItem?.Invoke(itemTypeStr);

        if (inventory[itemTypeStr] < long.MaxValue)
            inventory[itemTypeStr] += 1;
    }

}
