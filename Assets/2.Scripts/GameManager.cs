using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public QuestManager questManager;

    const string GoldKey = "Gold";
    public long gold;
    long maxGold = 9999999999; // 9,999,999,999

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Init();

        questManager.Init();

    }

    void Init()
    {
        gold = 0L;

        if (PlayerPrefs.HasKey(GoldKey))
        {
            string goldStr = PlayerPrefs.GetString(GoldKey);
            gold = long.TryParse(goldStr, out long result) ? result : 0L;
        }
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
        // player.Rest();
    }



}
