using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{

    public IslandData[] datas;
    public Dictionary<string, int[]> islandLevelDic;
    string IslandKey = "Island_";

    void Init()
    {
        // Island level 초기화
        for (int i = 0; i < datas.Length; i++)
        {
            string key = IslandKey + (i + 1); // key = Island_'n'
            int[] levelArr = new int[3];  // 각 level을 저장할 배열

            int farmLevel = PlayerPrefs.HasKey(key + "_FarmLevel") ? PlayerPrefs.GetInt(key + "_FarmLevel") : 1; // key = Island_'n'_FarmLevel
            int autoLevel = PlayerPrefs.HasKey(key + "_AutoLevel") ? PlayerPrefs.GetInt(key + "_AutoLevel") : 1; // key = Island_'n'_AutoLevel
            int cooldownLevel = PlayerPrefs.HasKey(key + "_CoolDownLevel") ? PlayerPrefs.GetInt(key + "_CoolDownLevel") : 1; // key = Island_'n'_CoolDownLevel

            islandLevelDic.Add(key, levelArr);
        }
    }

    public int GetUpgradeFarmGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + (islandIndex + 1)][0];

        return Mathf.FloorToInt(farmData.basicFarmGold * Mathf.Pow(farmData.farmGoldPer, level - 1)); // level 수정
    }

    public int GetUpgradeAutoProduceCanceGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + (islandIndex + 1)][1];

        return Mathf.FloorToInt(farmData.basicAutoProduceCanceGold * Mathf.Pow(farmData.autoProduceCanceGoldPer, level - 1)); // level 수정
    }

    public int GetUpgradeProduceCooldownGold(int islandIndex)
    {
        FarmData farmData = datas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + (islandIndex + 1)][2];

        return Mathf.FloorToInt(farmData.basicProduceCooldownGold * Mathf.Pow(farmData.produceCooldownGoldPer, level - 1)); // level 수정
    }

    public void SaveData()
    {
        // levelData
        for (int i = 0; i < datas.Length; i++)
        {
            string key = IslandKey + (i + 1); // key = Island_'n'
            int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

            PlayerPrefs.SetInt(key + "_FarmLevel", levelArr[0]); // key = Island_'n'_FarmLevel
            PlayerPrefs.SetInt(key + "_AutoLevel", levelArr[1]); // key = Island_'n'_AutoLevel
            PlayerPrefs.SetInt(key + "_CoolDownLevel", levelArr[2]); // key = Island_'n'_CoolDownLevel
        }

    }
}
