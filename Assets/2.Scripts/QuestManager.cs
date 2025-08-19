using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        GameManager.instance.player.onPlayerAction += OnPlayerAction;
    }

    void OnDisable()
    {
        GameManager.instance.player.onPlayerAction -= OnPlayerAction;
    }

    void OnPlayerAction(string actionName)
    {
        Debug.Log("QuestManager Player " + actionName);
    }

    public QuestData GetCurrentQuestData()
    {
        if (curQuestIndex >= datas.Length)
            return null;

        return datas[curQuestIndex];
    }
}
