using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
    - int index - 퀘스트 순서
    - string name - 퀘스트 이름
    - string desc - 퀘스트 설명
    - QuestCondition condition  - 퀘스트 조건
    - RewardsType reward - 퀘스트 보상
*/
[CreateAssetMenu(fileName = "Quest_", menuName = "ScriptableObject/Quest/Data")]
public class QuestData : ScriptableObject
{
    [Header("# Info")]
    public QuestType type;
    public int index;
    public string name;
    [TextArea] public string desc;

    [Header("# Condition")] 
    public QuestCondition condition;
    
    [Header("# Rewards")]
    public RewardsType rewardsType;
    public int rewardAmount;
}
