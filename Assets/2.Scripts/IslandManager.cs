using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IslandManager : MonoBehaviour
{

    public IslandData[] datas;
    public Dictionary<string, int[]> islandLevelDic;
    string IslandKey = "Island_";

    public void Init()
    {
        // Island level 초기화
        for (int i = 0; i < datas.Length; i++)
        {
            string key = IslandKey + i; // key = Island_'n'
            int[] levelArr = new int[3];  // 각 level을 저장할 배열

            int farmLevel = PlayerPrefs.HasKey(key + "_FarmLevel") ? PlayerPrefs.GetInt(key + "_FarmLevel") : 0; // key = Island_'n'_FarmLevel , 0 is unlock
            int autoLevel = PlayerPrefs.HasKey(key + "_AutoLevel") ? PlayerPrefs.GetInt(key + "_AutoLevel") : 0; // key = Island_'n'_AutoLevel , 0 is unlock
            int cooldownLevel = PlayerPrefs.HasKey(key + "_CoolDownLevel") ? PlayerPrefs.GetInt(key + "_CoolDownLevel") : 0; // key = Island_'n'_CoolDownLevel , 0 is unlock

            islandLevelDic.Add(key, levelArr);
        }
    }

    public void UnlockIsland(int islandIndex)
    {
        string key = IslandKey + islandIndex; // key = Island_'n'
        int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

        if (levelArr[0] != 1) return;

        // 언락되는 섬 level init
        levelArr[0] = 1;
        levelArr[1] = 1;
        levelArr[2] = 1;

        // 저장
        SaveData(islandIndex);
    }

    public void LevelUpFarm(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][0] += 1;

        SaveData(islandIndex);
    }

    public void LevelUpAutoProduceChance(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][1] += 1;

        SaveData(islandIndex);
    }

    public void LevelUpProduceCooldown(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][2] += 1;

        SaveData(islandIndex);
    }

    public int GetUpgradeFarmGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][0];

        return Mathf.FloorToInt(farmData.basicFarmGold * Mathf.Pow(farmData.farmGoldPer, level - 1)); // level 수정
    }

    public int GetUpgradeAutoProduceChanceGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][1];

        return Mathf.FloorToInt(farmData.basicAutoProduceChanceGold * Mathf.Pow(farmData.autoProduceChanceGoldPer, level - 1)); // level 수정
    }

    public int GetUpgradeProduceCooldownGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][2];

        return Mathf.FloorToInt(farmData.basicProduceCooldownGold * Mathf.Pow(farmData.produceCooldownGoldPer, level - 1)); // level 수정
    }

    public void SaveData(int islandIndex)
    {
        string key = IslandKey + islandIndex; // key = Island_'n'
        int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

        PlayerPrefs.SetInt(key + "_FarmLevel", levelArr[0]); // key = Island_'n'_FarmLevel
        PlayerPrefs.SetInt(key + "_AutoLevel", levelArr[1]); // key = Island_'n'_AutoLevel
        PlayerPrefs.SetInt(key + "_CoolDownLevel", levelArr[2]); // key = Island_'n'_CoolDownLevel
    }

    public void SaveDataAll()
    {
        // levelData
        for (int i = 0; i < datas.Length; i++)
        {
            string key = IslandKey + i; // key = Island_'n'
            int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

            PlayerPrefs.SetInt(key + "_FarmLevel", levelArr[0]); // key = Island_'n'_FarmLevel
            PlayerPrefs.SetInt(key + "_AutoLevel", levelArr[1]); // key = Island_'n'_AutoLevel
            PlayerPrefs.SetInt(key + "_CoolDownLevel", levelArr[2]); // key = Island_'n'_CoolDownLevel
        }
    }
}
