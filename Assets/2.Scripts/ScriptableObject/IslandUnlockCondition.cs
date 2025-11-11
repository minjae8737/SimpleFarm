using UnityEngine;

[CreateAssetMenu(fileName = "Quest_?_Cond", menuName = "ScriptableObject/Quest/Condition/IslandUnlock")]
public class IslandUnlockCondition : QuestCondition
{
    public int islandIndex;

    public override bool isSatisfied(QuestProgress progress)
    {
        return GameManager.instance.islandManager.IsUnlocked(islandIndex);
    }
}
