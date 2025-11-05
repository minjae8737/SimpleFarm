using UnityEngine;

[CreateAssetMenu(fileName = "Quest_?_Cond", menuName = "ScriptableObject/Quest/Condition/Behavior")]
public class BehaviourCondition : QuestCondition
{
    public string behaviourKey;
    public int targetCount;

    public override int GetTargetCount()
    {
        return targetCount;
    }
    
    public override bool isSatisfied(QuestProgress progress)
    {
        return  progress.curCount >= targetCount;
    }
}
