using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum QuestType { Behaviour, Produce, Drop }
public enum RewardsType { Gold }

public class QuestManager : MonoBehaviour
{
    public QuestData[] datas;
    string CurQuestIndexKey = "CurQuestIndex";
    int curQuestIndex; // 현재 진행중인 퀘스트
    string CurCountKey = "CurCount";
    public int curCount;  // 현재 퀘스트 목표횟수

    public event Action OnQuestProgressChanged;

    public void Init()
    {
        curQuestIndex = GameManager.instance.GetIntFromPlayerPrefs(CurQuestIndexKey);
        curCount = GameManager.instance.GetIntFromPlayerPrefs(CurCountKey);

        SetQuest();
    }

    private void OnApplicationQuit()
    {
        SaveQuest();
    }

    void OnDisable()
    {
        GameManager.instance.player.OnPlayerAction -= OnPlayerAction;
        GameManager.instance.inventory.OnItemAdded -= OnItemDrop;
    }

    void OnPlayerAction(string actionName)
    {
        Debug.Log("QuestManager Player " + actionName);
        OnQuestProgressChanged?.Invoke();
    }

    void OnItemDrop(ItemData itemdata,long quantity)
    {
        if (!itemdata.type.Equals(datas[curQuestIndex].requiredItemType))
            return;

        curCount++;
        OnQuestProgressChanged?.Invoke();

        if (CheckQuestCondition())
            ClearQuest();
    }

    public QuestData GetCurrentQuestData()
    {
        if (!CheckHaveQuestData())
            return null;

        return datas[curQuestIndex];
    }

    bool CheckHaveQuestData()
    {
        return curQuestIndex < datas.Length;
    }

    public void SetQuest()
    {
        if (curQuestIndex >= datas.Length)
            return;

        GameManager.instance.inventory.OnItemAdded -= OnItemDrop;
        GameManager.instance.player.OnPlayerAction -= OnPlayerAction;

        switch (datas[curQuestIndex].type)
        {
            case QuestType.Behaviour:
                GameManager.instance.player.OnPlayerAction += OnPlayerAction;
                break;
            case QuestType.Produce:
                break;
            case QuestType.Drop:
                GameManager.instance.inventory.OnItemAdded += OnItemDrop;
                break;
        }
    }

    public bool CheckQuestCondition()
    {
        if (curQuestIndex >= datas.Length)
            return false;

        return curCount >= datas[curQuestIndex].targetCount;
    }

    public void ClearQuest()
    {
        GameManager.instance.uiManager.OnQuestClearBtn();
    }

    public void GetReward()
    {
        // 리워드 지급
        switch (datas[curQuestIndex].rewardsType)
        {
            case RewardsType.Gold:
                GameManager.instance.SetGold(datas[curQuestIndex].rewardAmount);
                break;
        }

        curQuestIndex++;
        curCount = 0;

        if (CheckHaveQuestData())
        {
            SetQuest();
            GameManager.instance.uiManager.SetQuestPanel();
        }
        else
        {
            GameManager.instance.uiManager.OffQuestPanel();
        }

    }

    private void SaveQuest()
    {
        GameManager.instance.SaveIntToPlayerPrefs(CurQuestIndexKey, curQuestIndex);
        GameManager.instance.SaveIntToPlayerPrefs(CurCountKey, curCount);
    }
}
