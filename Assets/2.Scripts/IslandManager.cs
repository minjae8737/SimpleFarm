using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;


public class IslandManager : MonoBehaviour
{

    public IslandData[] islandDatas;
    public Dictionary<string, int[]> islandLevelDic;
    const int FARM_LEVEL_INDEX = 0;
    const int AUTOPRODUCECHANCE_LEVEL_INDEX = 1;
    const int PRODUCECOOLDOWN_LEVEL_INDEX = 2;
    string IslandKey = "Island_";
    
    public GameObject[] farmlands;
    public Dictionary<int, List<GameObject>> soils;
    public Dictionary<int, List<Produce>> crops;

    public GameObject[] cropPrefabs;
    
    public void Init()
    {
        islandLevelDic = new Dictionary<string, int[]>();
        soils = new Dictionary<int, List<GameObject>>();
        crops = new Dictionary<int, List<Produce>>();
        
        // Island level 초기화
        for (int i = 0; i < islandDatas.Length; i++)
        {
            string key = IslandKey + i; // key = Island_'n'
            int[] levelArr = new int[3];  // 각 level을 저장할 배열

            int farmLevel = PlayerPrefs.HasKey(key + "_FarmLevel") ? PlayerPrefs.GetInt(key + "_FarmLevel") : 0; // key = Island_'n'_FarmLevel , 0 is unlock
            int autoLevel = PlayerPrefs.HasKey(key + "_AutoLevel") ? PlayerPrefs.GetInt(key + "_AutoLevel") : 0; // key = Island_'n'_AutoLevel , 0 is unlock
            int cooldownLevel = PlayerPrefs.HasKey(key + "_CooldownLevel") ? PlayerPrefs.GetInt(key + "_CooldownLevel") : 0; // key = Island_'n'_CooldownLevel , 0 is unlock
            
            levelArr[FARM_LEVEL_INDEX] = farmLevel;
            levelArr[AUTOPRODUCECHANCE_LEVEL_INDEX] = autoLevel;
            levelArr[PRODUCECOOLDOWN_LEVEL_INDEX] = cooldownLevel;
            
            islandLevelDic.Add(key, levelArr);
        }

        // 첫번째 섬 초기화 
        UnlockIsland(0);
        
        // soil 객체 등록
        for (int i = 0; i < farmlands.Length; i++)
        {
            List<GameObject> soilList = new List<GameObject>();
            
            foreach (Transform child in farmlands[i].transform)
            {
                soilList.Add(child.gameObject);
            }

            soils.Add(i, soilList);
        }
        
        // crop 생성 및 데이터 초기화
        foreach (int key in soils.Keys)
        {
            IslandData islandData = islandDatas[key];
            FarmData farmData = islandData.farmData;
            
            List<GameObject> soilList = soils[key];
            List<Produce> cropList = new List<Produce>();
            int farmLevel = islandLevelDic[IslandKey + key][FARM_LEVEL_INDEX];
            
            float produceChance = GetAutoProduceChance(key, islandLevelDic[IslandKey + key][AUTOPRODUCECHANCE_LEVEL_INDEX]);
            float produceCooldown = GetProduceCooldown(key, islandLevelDic[IslandKey + key][PRODUCECOOLDOWN_LEVEL_INDEX]);
            
            for (int i = 0; i < farmLevel; i++)
            {
                GameObject newCrop = Instantiate(cropPrefabs[(int)farmData.farmType], soilList[i].transform);
                Produce produce = newCrop.GetComponent<Produce>();
                produce.SetDatas(produceChance,produceCooldown);
                cropList.Add(produce);
            }
            
            crops.Add(key, cropList);
        }
        
    }

    public void UnlockIsland(int islandIndex)
    {
        string key = IslandKey + islandIndex; // key = Island_'n'
        int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

        if (levelArr[FARM_LEVEL_INDEX] > 1 || levelArr[AUTOPRODUCECHANCE_LEVEL_INDEX] > 1 || levelArr[PRODUCECOOLDOWN_LEVEL_INDEX] > 1) return;

        // 언락되는 섬 level init
        levelArr[FARM_LEVEL_INDEX] = 1;
        levelArr[AUTOPRODUCECHANCE_LEVEL_INDEX] = 1;
        levelArr[PRODUCECOOLDOWN_LEVEL_INDEX] = 1;

        // 저장
        SaveData(islandIndex);
    }

    public void LevelUpFarm(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][FARM_LEVEL_INDEX] += 1;
        
        // crop 추가 로직
        int cropCount = crops[islandIndex].Count;
        List<GameObject> soilList = soils[islandIndex];
        List<Produce> cropList = crops[islandIndex];
        
        float produceChance = GetAutoProduceChance(islandIndex, islandLevelDic[IslandKey + islandIndex][AUTOPRODUCECHANCE_LEVEL_INDEX]);
        float produceCooldown = GetProduceCooldown(islandIndex, islandLevelDic[IslandKey + islandIndex][PRODUCECOOLDOWN_LEVEL_INDEX]);

        GameObject newCrop = Instantiate(cropPrefabs[(int)farmData.farmType], soilList[cropCount].transform);
        Produce produce = newCrop.GetComponent<Produce>();
        produce.SetDatas(produceChance, produceCooldown);
        cropList.Add(produce);
        
        SaveData(islandIndex);
    }

    public void LevelUpAutoProduceChance(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][AUTOPRODUCECHANCE_LEVEL_INDEX] += 1;
        
        // autoProduce 재적용 로직
        List<Produce> cropList = crops[islandIndex];
        float autoProduceChance = GetAutoProduceChance(islandIndex, islandLevelDic[IslandKey + islandIndex][AUTOPRODUCECHANCE_LEVEL_INDEX]);

        foreach (Produce produce in cropList)
        {
            produce.SetAutoProduce(autoProduceChance);
        }
        
        SaveData(islandIndex);
    }

    public void LevelUpProduceCooldown(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;

        islandLevelDic[IslandKey + islandIndex][PRODUCECOOLDOWN_LEVEL_INDEX] += 1;
        
        // cooldown 재적용 로직
        List<Produce> cropList = crops[islandIndex];
        float produceCooldown = GetProduceCooldown(islandIndex, islandLevelDic[IslandKey + islandIndex][PRODUCECOOLDOWN_LEVEL_INDEX]);

        foreach (Produce produce in cropList)
        {
            produce.SetCoolTime(produceCooldown);
        }
        
        SaveData(islandIndex);
    }

    public long GetUpgradeFarmGold(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][FARM_LEVEL_INDEX];
        
        double result = farmData.basicFarmGold * Mathf.Pow(farmData.farmGoldPer, level - 1); // level 수정
        return (long)Math.Floor(result); 
    }

    public long GetUpgradeAutoProduceChanceGold(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][AUTOPRODUCECHANCE_LEVEL_INDEX];

        double result = farmData.basicAutoProduceChanceGold * Mathf.Pow(farmData.autoProduceChanceGoldPer, level - 1);
        return (long)Math.Floor(result); 
    }

    public long GetUpgradeProduceCooldownGold(int islandIndex)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;
        int level = islandLevelDic[IslandKey + islandIndex][PRODUCECOOLDOWN_LEVEL_INDEX];
        
        double result = farmData.basicProduceCooldownGold * Mathf.Pow(farmData.produceCooldownGoldPer, level - 1);
        return (long)Math.Floor(result); 
    }

    public float GetAutoProduceChance(int islandIndex, int level)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;

        return farmData.basicAutoProduceChance + (farmData.autoProduceChancePer * (level - 1));
    }
    
    public float GetProduceCooldown(int islandIndex, int level)
    {
        FarmData farmData = islandDatas[islandIndex].farmData;
        
        return farmData.basicProduceCooldown - (farmData.produceCooldownPer * (level - 1));
    }

    public FarmUpgradePanelDTO GetFarmUpgradePanelDTO(int islandIndex)
    {
        IslandData islandData = islandDatas[islandIndex];
        FarmData farmData = islandData.farmData;
        int[] islandLevelArr = islandLevelDic[IslandKey + islandIndex];

        return new FarmUpgradePanelDTO
        {
            islandType = islandData.islandType,
            farmTitle = farmData.name,
            maxFarmLevel = farmData.maxFarmLevel,
            farmLevel = islandLevelArr[FARM_LEVEL_INDEX],
            farmCurrentValue = islandLevelArr[FARM_LEVEL_INDEX],
            farmNextValue = islandLevelArr[FARM_LEVEL_INDEX] + 1,
            farmGold = GetUpgradeFarmGold(islandIndex),
            autoLevel = islandLevelArr[AUTOPRODUCECHANCE_LEVEL_INDEX],
            maxAutoLevel = farmData.maxAutoProduceChanceLevel,
            autoCurrentValue = GetAutoProduceChance(islandIndex, islandLevelArr[AUTOPRODUCECHANCE_LEVEL_INDEX]),
            autoNextValue = GetAutoProduceChance(islandIndex, islandLevelArr[AUTOPRODUCECHANCE_LEVEL_INDEX] + 1),
            autoGold = GetUpgradeAutoProduceChanceGold(islandIndex),
            cooldownLevel = islandLevelArr[PRODUCECOOLDOWN_LEVEL_INDEX],
            maxCooldownLevel = farmData.maxProduceCooldownLevel,
            cooldownCurrentValue = GetProduceCooldown(islandIndex, islandLevelArr[PRODUCECOOLDOWN_LEVEL_INDEX]),
            cooldownNextValue = GetProduceCooldown(islandIndex, islandLevelArr[PRODUCECOOLDOWN_LEVEL_INDEX] + 1),
            cooldownGold = GetUpgradeProduceCooldownGold(islandIndex),
        };
    }
    
    public void SaveData(int islandIndex)
    {
        string key = IslandKey + islandIndex; // key = Island_'n'
        int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

        PlayerPrefs.SetInt(key + "_FarmLevel", levelArr[FARM_LEVEL_INDEX]); // key = Island_'n'_FarmLevel
        PlayerPrefs.SetInt(key + "_AutoLevel", levelArr[AUTOPRODUCECHANCE_LEVEL_INDEX]); // key = Island_'n'_AutoLevel
        PlayerPrefs.SetInt(key + "_CooldownLevel", levelArr[PRODUCECOOLDOWN_LEVEL_INDEX]); // key = Island_'n'_CooldownLevel
    }

    public void SaveDataAll()
    {
        // levelData
        for (int i = 0; i < islandDatas.Length; i++)
        {
            string key = IslandKey + i; // key = Island_'n'
            int[] levelArr = islandLevelDic[key];  // 각 level을 저장한 배열

            PlayerPrefs.SetInt(key + "_FarmLevel", levelArr[FARM_LEVEL_INDEX]); // key = Island_'n'_FarmLevel
            PlayerPrefs.SetInt(key + "_AutoLevel", levelArr[AUTOPRODUCECHANCE_LEVEL_INDEX]); // key = Island_'n'_AutoLevel
            PlayerPrefs.SetInt(key + "_CooldownLevel", levelArr[PRODUCECOOLDOWN_LEVEL_INDEX]); // key = Island_'n'_CooldownLevel
        }
    }
}
