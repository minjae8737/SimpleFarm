using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
    - int questNum - 퀘스트 순서
    - string title - 퀘스트 제목
    - string desc - 퀘스트 설명
    - ? reward - 퀘스트 보상
    - int  - 퀘스트 조건
    - int  - 현재 조건
*/
[CreateAssetMenu(fileName = "Quest_", menuName = "ScriptableObject/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("# Info")]
    public QuestType type;
    public int index;
    public string name;
    [TextArea]
    public string desc;
    public ItemType requiredItemType;
    public int targetCount;
    [Header("# Rewards")]
    public RewardsType rewardsType;
    public int rewardAmount;
}
