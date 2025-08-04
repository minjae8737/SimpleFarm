using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestData[] datas;
    string CurQuestIndexKey = "CurQuestIndex";
    int curQuestIndex; // 현재 진행중인 퀘스트
    string CurCountKey = "CurCount";
    int curCount;  // 현재 퀘스트 목표횟수

    void OnDisable()
    {
        GameManager.instance.player.onPlayerAction -= OnPlayerAction;
    }

    public void Init()
    {
        curQuestIndex = 0;

        if (PlayerPrefs.HasKey(CurQuestIndexKey))
        {
            curQuestIndex = PlayerPrefs.GetInt(CurQuestIndexKey);
        }

        curCount = 0;

        if (PlayerPrefs.HasKey(CurCountKey))
        {
            curCount = PlayerPrefs.GetInt(CurCountKey);
        }

        GameManager.instance.player.onPlayerAction += OnPlayerAction;
    }

    void OnPlayerAction(string actionName)
    {
        Debug.Log("QuestManager Player " + actionName);
    }

}
