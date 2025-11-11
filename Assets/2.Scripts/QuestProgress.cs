using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestProgress 
{
    public QuestData questData;
    public int curQuestIndex; // 현재 진행중인 퀘스트
    public int curCount;  // 현재 퀘스트 달성한 카운트
    

    public int GetCurCount()
    {
        return curCount; 
    }
    
    public bool IsSatisfied()
    {
        return questData.condition.isSatisfied(this);
    }
}
