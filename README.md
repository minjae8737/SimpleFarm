# 🎮 곡괭이 왕 아일랜드 (Clone Project)

타이쿤 게임 **“곡괭이 왕 아일랜드”**를 플레이해보고, 이를 직접 만들어보고 싶어 진행한 모작 프로젝트입니다.

---

## 🚀 프로젝트 소개

타이쿤 게임의 기본 구조(생산 → 수확 → 업그레이드 → 확장)를 이해하고,
실제 기능 구현 중심으로 시스템 설계 및 개발 역량을 강화하기 위해 진행한 프로젝트입니다.

---

## 🛠 작업 환경

- **OS** : `Mac OS 15.5`  
- **Engine** : `Unity 2022.3.62f2`  
- **IDE** : `Rider`  
- **Version Control** : `Git`, `GitHub`, `Sourcetree`  
- **Assets / Tools** : `TextMeshPro`, `DOTween`

---

## 📅 작업 기간
2025.07.24 ~ 2025.11.11

## 🎥 플레이 영상

[YouTube 링크](https://www.youtube.com/watch?v=Oa23-0gLe24)

---

## 📌 주요 구현 내용

## 🌱 농장 시스템
- `ScriptableObject` 기반 데이터 구조화
- 생산 / 수확 / 자동수확 로직 구현
- 농장 업그레이드 시스템 구현


## 📜 퀘스트 시스템
- `ScriptableObject` 기반 데이터 설계
- `QuestCondition`을 활용한 다형성 구조 적용
- 다양한 조건 클래스를 추가 확장할 수 있는 구조 구축

<details>
<summary>🔧 코드 예시</summary>
  
```csharp
// ScriptableObject로 Quest 데이터구조 설계
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

// QuestCondition 다형성 구조
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
        return progress.curCount >= targetCount;
    }
}

public class IslandUnlockCondition : QuestCondition
{
    public int islandIndex;

    public override bool isSatisfied(QuestProgress progress)
    {
        return GameManager.instance.islandManager.IsUnlocked(islandIndex);
    }
}

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
        return progress.curCount >= targetCount;
    }
}
```
</details>

## 🎒 인벤토리 시스템
- 초기화 시 Dictionary로 아이템 빠른 조회 구조 구성
- UI와의 결합도를 낮추기 위해 이벤트 콜백 사용
- 최대 스택 제한 처리 포함

<details>
<summary>🔧 코드 예시</summary>

```csharp
public class Inventory
{
    private Dictionary<string, long> items;

    public Dictionary<string, long> Items => items;

    const string ItemKey = "Item_";
    
    public event Action<ItemData, long> OnItemAdded;
    public event Action<ItemData, long> OnItemRemoved;
    
    public void Init()
    {
        items = new Dictionary<string, long>();
        
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            string key = ItemKey + type; // "Item_xxxx"
            items.Add(key, GameManager.instance.GetLongFromPlayerPrefs(key));
        }
    }

    public void AddItem(ItemData itemData, long quantity)
    {
        if (items.ContainsKey(ItemKey + itemData.type))
        {
            items[ItemKey + itemData.type] += quantity;
            if (items[ItemKey + itemData.type] > itemData.maxStackSize)
                items[ItemKey + itemData.type] = itemData.maxStackSize;
        }
        else
        {
            items.Add(ItemKey + itemData.type, quantity);
        }

        OnItemAdded?.Invoke(itemData, quantity);
    }

    public void RemoveItem(ItemData itemData, long quantity)
    {
        if (items.ContainsKey(ItemKey + itemData.type))
        {
            if (items[ItemKey + itemData.type] - quantity < 0)
                return;
            
            items[ItemKey + itemData.type] -= quantity;
            OnItemRemoved?.Invoke(itemData, quantity);
        }
    }

    public void RemoveItem(ItemType itemType, long quantity)
    {
        ItemData itemData = GameManager.instance.GetItemData(itemType);
        
        if (items.ContainsKey(ItemKey + itemData.type))
        {
            if (items[ItemKey + itemData.type] - quantity < 0)
                return;
            
            items[ItemKey + itemData.type] -= quantity;
            OnItemRemoved?.Invoke(itemData, quantity);
        }
    }
}
```
  
</details>

## 🎛 오브젝트 풀링
- 드랍 아이템 생성 비용 절감
- AudioSource 풀을 이용한 SFX 재생 최적화

---
