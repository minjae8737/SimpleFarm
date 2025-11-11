using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    public virtual ItemType GetItemType()
    {
        return ItemType.None;
    }

    public virtual int GetTargetCount()
    {
        return int.MaxValue;
    }

    public abstract bool isSatisfied(QuestProgress progress);
}
