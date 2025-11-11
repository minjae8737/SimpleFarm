using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest_?_Cond", menuName = "ScriptableObject/Quest/Condition/Produce")]
public class ProduceCondition : QuestCondition
{
    public ItemType itemType;
    public int targetCount;
    
    public override ItemType GetItemType()
    {
        return itemType;
    }
    
    public override int GetTargetCount()
    {
        return targetCount;
    }
    
    public override bool isSatisfied(QuestProgress progress)
    {
        return  progress.curCount >= targetCount;
    }
}
