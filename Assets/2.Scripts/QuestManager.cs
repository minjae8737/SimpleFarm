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
    }

    void OnItemDrop(string itemType)
    {
        if (!itemType.Equals(datas[curQuestIndex].requiredObjectType.ToString()))
            return;

        Debug.Log("Drop Item Name " + itemType);
        curCount++;
    }

    public QuestData GetCurrentQuestData()
    {
        if (curQuestIndex >= datas.Length)
            return null;

        return datas[curQuestIndex];
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

        return curCount >= datas[curQuestIndex].rewardAmount;
    }

    public void ClearQuest()
    {
        
    }

}
