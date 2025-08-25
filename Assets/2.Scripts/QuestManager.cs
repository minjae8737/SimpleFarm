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

    public event Action refreshQuestInfoEvent;

    public void Init()
    {
        curQuestIndex = PlayerPrefs.HasKey(CurQuestIndexKey) ? PlayerPrefs.GetInt(CurQuestIndexKey) : 0;
        curCount = PlayerPrefs.HasKey(CurCountKey) ? PlayerPrefs.GetInt(CurCountKey) : 0;

        SetQuest();
    }

    void OnDisable()
    {
        GameManager.instance.player.onPlayerAction -= OnPlayerAction;
        GameManager.instance.pickedItem -= OnItemDrop;
    }

    void OnPlayerAction(string actionName)
    {
        Debug.Log("QuestManager Player " + actionName);
        refreshQuestInfoEvent?.Invoke();
    }

    void OnItemDrop(string itemType)
    {
        if (!itemType.Equals(datas[curQuestIndex].requiredObjectType.ToString()))
            return;

        Debug.Log("Drop Item Name " + itemType);
        curCount++;
        refreshQuestInfoEvent?.Invoke();

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

        switch (datas[curQuestIndex].type)
        {
            case QuestType.Behaviour:
                GameManager.instance.player.onPlayerAction += OnPlayerAction;
                break;
            case QuestType.Produce:
                break;
            case QuestType.Drop:
                GameManager.instance.pickedItem += OnItemDrop;
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



}
