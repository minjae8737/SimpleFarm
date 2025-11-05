using UnityEngine;

[CreateAssetMenu(fileName = "Quest_?_Cond", menuName = "ScriptableObject/Quest/Condition/Collect")]
public class CollectItemCondition : QuestCondition
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
