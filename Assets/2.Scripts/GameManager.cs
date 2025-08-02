using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    const string GoldKey = "Gold";
    public long gold;
    long maxGold = 9999999999; // 9,999,999,999

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Init();
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

    void SaveGold()
    {
        PlayerPrefs.SetString(GoldKey, gold.ToString());
    }

    void RestPlayer()
    {
        // player.Rest();
    }



}
