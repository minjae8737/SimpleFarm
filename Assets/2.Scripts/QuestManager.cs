using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum QuestType { Behaviour, Produce, CollectItem, IslandUnlocked }
public enum RewardsType { Gold }

public class QuestManager : MonoBehaviour
{
    public QuestData[] quests;
    private string CurQuestIndexKey = "CurQuestIndex";
    private string CurCountKey = "CurCount";
    private QuestProgress progress;
    public QuestProgress Progress => progress;
    public int CurCount => progress.curCount;
    public int TargetCount => progress.questData.condition.GetTargetCount();

    public event Action OnQuestProgressChanged;

    public void Init()
    {
        progress = new QuestProgress();
        
        progress.curQuestIndex = GameManager.instance.GetIntFromPlayerPrefs(CurQuestIndexKey);
        progress.curCount = GameManager.instance.GetIntFromPlayerPrefs(CurCountKey);
        progress.questData = progress.curQuestIndex < quests.Length ? quests[progress.curQuestIndex] : null;

        SetQuest();
    }

    private void Start()
    {
        if (CheckQuestCondition())
            ClearQuest();
    }

    private void OnApplicationQuit()
    {
        SaveQuest();
    }

    void OnDisable()
    {
        GameManager.instance.player.OnPlayerAction -= OnPlayerAction;
        GameManager.instance.inventory.OnItemAdded -= OnItemDrop;
        GameManager.instance.islandManager.OnIslandUnlocked -= OnIslandUnlocked;
    }

    void OnPlayerAction(string actionName)
    {
        Debug.Log("QuestManager Player " + actionName);
        OnQuestProgressChanged?.Invoke();
    }

    void OnItemDrop(ItemData itemdata,long quantity)
    {
        if (!itemdata.type.Equals(progress.questData.condition.GetItemType()))
            return;

        progress.curCount++;
        OnQuestProgressChanged?.Invoke();

        if (CheckQuestCondition())
            ClearQuest();
    }
    
    private void OnIslandUnlocked()
    {
        if (CheckQuestCondition())
            ClearQuest();
    }

    private bool CheckHaveQuestData()
    {
        return progress.curQuestIndex < quests.Length;
    }

    public void SetQuest()
    {
        if (progress.curQuestIndex >= quests.Length)
            return;

        GameManager.instance.inventory.OnItemAdded -= OnItemDrop;
        GameManager.instance.player.OnPlayerAction -= OnPlayerAction;
        GameManager.instance.islandManager.OnIslandUnlocked -= OnIslandUnlocked;

        switch (progress.questData.type)
        {
            case QuestType.Behaviour:
                GameManager.instance.player.OnPlayerAction += OnPlayerAction;
                break;
            case QuestType.Produce:
                break;
            case QuestType.CollectItem:
                GameManager.instance.inventory.OnItemAdded += OnItemDrop;
                break;
            case QuestType.IslandUnlocked:
                GameManager.instance.islandManager.OnIslandUnlocked += OnIslandUnlocked;
                break;
        }
    }

    public bool CheckQuestCondition()
    {
        if (progress.curQuestIndex >= quests.Length)
            return false;
        
        return progress.questData.condition.isSatisfied(progress);
    }

    public void ClearQuest()
    {
        GameManager.instance.uiManager.OnQuestClearBtn();
    }

    public void GetReward()
    {
        // 리워드 지급
        switch (progress.questData.rewardsType)
        {
            case RewardsType.Gold:
                GameManager.instance.SetGold(progress.questData.rewardAmount);
                break;
        }

        progress.curQuestIndex++;
        progress.curCount = 0;
        progress.questData = progress.curQuestIndex < quests.Length ? quests[progress.curQuestIndex] : null;

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
        GameManager.instance.SaveIntToPlayerPrefs(CurQuestIndexKey, progress.curQuestIndex);
        GameManager.instance.SaveIntToPlayerPrefs(CurCountKey, progress.curCount);
    }
}
