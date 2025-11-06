using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIBtnType
{
    RecoverHP,
    Shop,
    Island_Wheat,
    Island_Beet,
    Island_Cabbage,
    Island_Carrot,
    Island_Cauliflower,
}

public class UIManager : MonoBehaviour
{
    [Header("HUD")] 
    [SerializeField] private Text goldText;
    
    [Space(5)]
    [Header("QuestPanel")]
    [SerializeField] private GameObject questPanel;
    private Image questRewardIcon;
    private Text questRewardText;
    private Text questDesc;
    private GameObject questClearBtn;
    private Image questClearBtnRewardIcon;
    private Text questClearBtnRewardText;
    private Text questClearBtnDesc;
    
    [Space(5)]
    [Header("PlayerHp")]
    [SerializeField] private GameObject playerHp;
    private Image palyerHpImg;
    private Text playerHpText;
    private float recoveHpDuration = 1f; // FillAmount 0 -> 1 까지 걸리는 시간
    
    [Space(5)]
    [Header("InteractButton")]
    [SerializeField] private Button interactUIBtn;
    private Text interactUIBtnText;
    
    [Space(5)]
    [Header("ActionButton")]
    public ActionBtn actionBtn;

    [SerializeField] private Sprite[] actionBtnIconSprites;

    [Header("Menu")] 
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private Button magnetBtn;
    private TextMeshProUGUI magnetBtnText;

    [Header("Shop")] 
    [SerializeField] private GameObject shopPanel;
    private RectTransform shopContent;
    [SerializeField] private GameObject shopItemPrefab;
    private List<GameObject> shopItems;
    public ItemSellPanel itemSellPanel;

    [Header("Item")] 
    [SerializeField] private GameObject itemInfoPanel;
    private Text itemNameText;
    private Image itemIcon;
    private Text itemPriceText;
    private Text itemQuantityText;
    private Text itemDescriptionText;

    [Header("Farm")]
    [SerializeField] private FarmUpgradePanel farmUpgradePanel;
    
    [Header("Inventory")]
    [SerializeField] private InventoryPanel inventoryPanel;
    
    [Header("AddIslandBtn")]
    [SerializeField] private AddIslandBtn addIslandBtn;
    
    [Header("FloatingText")]
    [SerializeField] private Transform floatingTextPool;
    [SerializeField] private int floatingTextPoolSize;
    [SerializeField] private List<FloatingText> floatingTexts;
    [SerializeField] private GameObject floatingTextPrefab;
    
    [Header("Speech Bubble")]
    [SerializeField] private SpeechBubble playerSpeechBubble;

    private Dictionary<string, string> playerLinesDic = new Dictionary<string, string>()
    {
        { "LowHp", "잠깐 쉴래" },
    };
    
    [Header("Sprites")] 
    [SerializeField] private Sprite[] rewardIcons; // RewardsType과 매칭

    public void Init()
    {
        palyerHpImg = playerHp.transform.GetChild(0).GetComponent<Image>();
        playerHpText = playerHp.transform.GetChild(1).GetComponent<Text>();

        // 퀘스트 리워드
        Transform questReward = questPanel.transform.GetChild(0);
        questRewardIcon = questReward.GetChild(0).GetComponent<Image>();
        questRewardText = questReward.GetChild(1).GetComponent<Text>();
        questDesc = questPanel.transform.GetChild(1).GetComponent<Text>();

        // 퀘스트 클리어 버튼
        questClearBtn = questPanel.transform.GetChild(2).gameObject;
        Transform questClearReward = questClearBtn.transform.GetChild(0);
        questClearBtnRewardIcon = questClearReward.GetChild(0).GetComponent<Image>();
        questClearBtnRewardText = questClearReward.GetChild(1).GetComponent<Text>();
        questClearBtnDesc = questClearBtn.transform.GetChild(1).GetComponent<Text>();

        // 상호작용 버튼
        interactUIBtnText =  interactUIBtn.transform.GetChild(0).GetComponent<Text>();
        
        // 상점
        shopContent = shopPanel.transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();

        // 아이템
        itemNameText = itemInfoPanel.transform.GetChild(0).GetComponent<Text>();
        itemIcon = itemInfoPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        itemPriceText = itemInfoPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        itemQuantityText = itemInfoPanel.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        itemDescriptionText = itemInfoPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        
        // 메뉴 버튼
        magnetBtnText =  magnetBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        // 플로팅 텍스트
        floatingTexts = new List<FloatingText>();
        
        SetPlayerHp();
        RefreshGoldText();
        SetQuestPanel();
        InitShopItems();
        InitInventoryPanel();
        InitAddIslandBtn();
        InitFloatingText();
        
        itemSellPanel.OffItemInfoPanel += OffItemInfoPanel;
        
        farmUpgradePanel.OnClickFarmUpgradeBtn += OnClickFarmUpgradeBtn;
        farmUpgradePanel.OnClickAutoUpgradeBtn += OnClickAutoUpgradeBtn;
        farmUpgradePanel.OnClickCooldownUpgradeBtn += OnClickCooldownUpgradeBtn;
    }

    public void PlayUIOpenSfx()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.UIOpen);
    }

    public void PlayUICloseSfx()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.UIClose);
    }
    
    public void PlayUIButtonClickEnabledSfx()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.UIButtonClickEnabled);
    }

    public void PlayUIButtonClickDisabledSfx()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.UIButtonClickDisabled);
    }

    #region Gold

    public void RefreshGoldText()
    {
        goldText.text = ConvertGoldToText(GameManager.instance.gold);
    }

    public string ConvertGoldToText(long gold)
    {
        string goldStr = "";
        string[] postFixUnit = { "", "K", "M", "B" };

        int count = 0;
        double goldD = gold;

        while (goldD >= 1000)
        {
            goldD /= 1000;
            count++;
        }

        if (count >= postFixUnit.Length)
            count = postFixUnit.Length - 1;

        goldD = Math.Floor(goldD * 100) / 100; // 반올림 방지
        goldStr = string.Format("{0:0.##}", goldD) + postFixUnit[count];

        return goldStr;
    }

    #endregion

    public void SetPlayerHp(string defaultPlayerAction = "")
    {
        float playerHpAmount = (float)GameManager.instance.player.Hp / GameManager.instance.player.MaxHp;
        palyerHpImg.fillAmount = playerHpAmount;

        SetPlayerHpText();
    }

    void SetPlayerHpText()
    {
        playerHpText.text = GameManager.instance.player.Hp + " / " + GameManager.instance.player.MaxHp;
    }

    public void RecorvePlayerHpEffect()
    {
        OffHpbarText();

        Sequence sequence = DOTween.Sequence();

        float effectSpeed = 1f / recoveHpDuration;
        float fillTime = (1f - palyerHpImg.fillAmount) / effectSpeed;

        Tween fillHpTween = palyerHpImg.DOFillAmount(1f, fillTime).SetEase(Ease.Linear);
        Tween scaleBiggerTween = playerHp.GetComponent<RectTransform>().DOScale(1.2f, 0.5f);
        Tween scaleSmallerTween = playerHp.GetComponent<RectTransform>().DOScale(1f, 0.5f);

        sequence.Append(fillHpTween)
            .Join(scaleBiggerTween)
            .Append(scaleSmallerTween)
            .OnComplete(OnCompleteRecorvePlayerHpEffect);
    }

    void OnCompleteRecorvePlayerHpEffect()
    {
        SetPlayerHpText();
        OnHpbarText();
        GameManager.instance.player.OffSleepEffect();
    }

    public void SetQuestPanel()
    {
        QuestProgress questProgress = GameManager.instance.questManager.Progress;

        if (questProgress.questData == null)
        {
            questPanel.SetActive(false);
            return;
        }
        
        string questDescStr = "";

        switch (questProgress.questData.type)
        {
            case QuestType.Behaviour:
            case QuestType.Produce:
            case QuestType.CollectItem:
                questDescStr = string.Format("{0} [{1}/{2}]", questProgress.questData.desc, GameManager.instance.questManager.CurCount, GameManager.instance.questManager.TargetCount);
                break;
            case QuestType.IslandUnlocked:
                questDescStr = string.Format("{0}", questProgress.questData.desc);
                break;
        }
        
        string questRewardAmountStr = ConvertGoldToText(questProgress.questData.rewardAmount);
        
        questDesc.text = questDescStr;
        questRewardIcon.sprite = rewardIcons[(int)questProgress.questData.rewardsType];
        questRewardText.text = questRewardAmountStr;

        questClearBtnDesc.text = questDescStr;
        questClearBtnRewardIcon.sprite = rewardIcons[(int)questProgress.questData.rewardsType];
        questClearBtnRewardText.text = questRewardAmountStr;
    }

    public void OnQuestPanel()
    {
        questPanel.SetActive(true);
    }

    public void OffQuestPanel()
    {
        questPanel.SetActive(false);
    }

    void OnHpbarText()
    {
        playerHpText.gameObject.SetActive(true);
    }

    void OffHpbarText()
    {
        playerHpText.gameObject.SetActive(false);
    }

    public void SetInteractBtn(UIBtnType type)
    {
        interactUIBtn.onClick.RemoveAllListeners();

        switch (type)
        {
            case UIBtnType.RecoverHP:
                interactUIBtnText.text = "휴식 하기";
                interactUIBtn.onClick.AddListener(GameManager.instance.RestPlayer);
                break;
            case UIBtnType.Shop:
                interactUIBtnText.text = "상점 열기";
                interactUIBtn.onClick.AddListener(OpenShop);
                break;
            case UIBtnType.Island_Wheat:
                interactUIBtnText.text = "밀밭을 사고 싶어!";
                interactUIBtn.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Wheat));
                break;
            case UIBtnType.Island_Beet:
                interactUIBtnText.text = "비트밭을 사고 싶어!";
                interactUIBtn.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Beet));
                break;
            case UIBtnType.Island_Cabbage:
                interactUIBtnText.text = "양배추밭을 사고 싶어!";
                interactUIBtn.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Cabbage));
                break;
            case UIBtnType.Island_Carrot:
                interactUIBtnText.text = "당근밭을 사고 싶어!";
                interactUIBtn.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Carrot));
                break;
            case UIBtnType.Island_Cauliflower:
                interactUIBtnText.text = "컬리플라워밭을 사고 싶어!";
                interactUIBtn.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Cauliflower));
                break;
        }

        OnInteractBtnEffect();
    }

    void OnInteractBtnEffect()
    {
        if (interactUIBtn == null)
            return;

        interactUIBtn.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        Tween scaleBiggerTween = interactUIBtn.GetComponent<RectTransform>().DOScale(1.02f, 0.2f);
        Tween scaleSmallerTween = interactUIBtn.GetComponent<RectTransform>().DOScale(1f, 0.2f);

        sequence.Append(scaleBiggerTween)
            .Append(scaleSmallerTween);
    }

    public void OffInteractBtn()
    {
        if (interactUIBtn == null)
            return;

        interactUIBtn.gameObject.SetActive(false);
    }

    void OnClickInteractBtn()
    {
        interactUIBtn.onClick?.Invoke();
    }

    public void OnQuestClearBtn()
    {
        questClearBtn.SetActive(true);
    }

    void OffQuestClearBtn()
    {
        questClearBtn.SetActive(false);
    }

    public void OnClickQuestClearBtn()
    {
        GameManager.instance.questManager.GetReward();
        OffQuestClearBtn();
        PlayUIButtonClickEnabledSfx();
    }

    #region Shop

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        shopPanel.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
        PlayUIOpenSfx();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        PlayUICloseSfx();
    }

    public void InitShopItems()
    {
        shopItems = new List<GameObject>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if(type == ItemType.None) continue;
            
            GameObject newShopItem = Instantiate(shopItemPrefab, shopContent);
            shopItems.Add(newShopItem);
            ShopItem shopItem = newShopItem.GetComponent<ShopItem>();
            shopItem.Init(GameManager.instance.itemDatas[(int)type]);
            shopItem.OnClickShopItem += OnItemSellPanel;
        }
    }

    public void RefreshShopItem(ItemData itemData, long quantity)
    {
        GameObject findShopItem = shopItems.Find(item => item.GetComponent<ShopItem>().itemName == itemData.itemName);
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());
        findShopItem?.GetComponent<ShopItem>().RefreshQuantity(itemQuantity);
    }

    #endregion

    #region ItemSellPanel

    void OnItemSellPanel(ItemData itemData)
    {
        SetItemSellPanel(itemData);
        itemSellPanel.gameObject.SetActive(true);
        OnItemInfoPanel(itemData);
    }

    void SetItemSellPanel(ItemData itemData)
    {
        itemSellPanel.Init(itemData, this);
    }

    #endregion

    #region ItemInfoPanel

    public void OnItemInfoPanel(ItemData itemData)
    {
        SetItemInfoPanel(itemData);
        itemInfoPanel.SetActive(true);
        itemInfoPanel.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
        PlayUIOpenSfx();
    }

    void OffItemInfoPanel()
    {
        itemInfoPanel.SetActive(false);
        PlayUICloseSfx();
    }

    void SetItemInfoPanel(ItemData itemData)
    {
        itemNameText.text = itemData.itemName;
        itemIcon.sprite = itemData.icon;
        itemPriceText.text = "가격:" + itemData.price;
        itemQuantityText.text = "수량:x" + GameManager.instance.inventory.GetItemQuantity(itemData.type.ToString());
        itemDescriptionText.text = itemData.description;
    }

    #endregion
    
    #region FarmUpgradePanel

    public void OnFarmUpgradePanel()
    {
        farmUpgradePanel.gameObject.SetActive(true);
        PlayUIOpenSfx();
    }

    public void OffFarmUpgradePanel()
    {
        farmUpgradePanel.gameObject.SetActive(false);
        PlayUICloseSfx();
    }
    
    private void SetFarmUpgradePanel(IslandType islandType)
    {
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
        OnFarmUpgradePanel();
    }

    private void OnClickFarmUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold))
        {
            PlayUIButtonClickDisabledSfx();
            return;
        }
        
        GameManager.instance.islandManager.LevelUpFarm((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
        PlayUIButtonClickEnabledSfx();

    }

    private void OnClickAutoUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold))
        {
            PlayUIButtonClickDisabledSfx();
            return;
        }
        
        GameManager.instance.islandManager.LevelUpAutoProduceChance((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
        PlayUIButtonClickEnabledSfx();
    }

    private void OnClickCooldownUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold))
        {
            PlayUIButtonClickDisabledSfx();
            return;
        }
        
        GameManager.instance.islandManager.LevelUpProduceCooldown((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
        PlayUIButtonClickEnabledSfx();
    }
    
    #endregion

    #region InventoryPanel

    void InitInventoryPanel()
    {
        inventoryPanel.Init();
    }
    
    public void RefreshInventoryPanel(ItemData itemData, long quantity)
    {
        inventoryPanel.RefreshInventoryPanel(itemData);
    }

    public void OnInventoryPanel()
    {
        questPanel.SetActive(false);
        playerHp.SetActive(false);
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
        PlayUIOpenSfx();
    }

    public void OffInventoryPanel()
    {
        inventoryPanel.gameObject.SetActive(false);
        questPanel.SetActive(true);
        playerHp.SetActive(true);
        PlayUICloseSfx();
    }

    #endregion
    
    #region MenuBtn

    public void OnClickMagnetBtn(TimerHandler timerHandler)
    {
        StartCoroutine(StartUpdateText(timerHandler));
    }

    private IEnumerator StartUpdateText(TimerHandler timerHandler)
    {
        while (!GameManager.instance.canPickUp)
        {
            magnetBtnText.text = string.Format("{0:0.#}s", timerHandler.TimeLimit);
            yield return null;
        }

        magnetBtnText.text = "READY";
    }
    
    #endregion

    #region AddIslandBtn

    private void InitAddIslandBtn()
    {
        addIslandBtn.OnClick += OnclickAddIslandBtn;
        SetAddIslandBtn();
    }
    
    private void SetAddIslandBtn()
    {
        int currentIslandLevel = GameManager.instance.islandManager.GetCurrentIslandLevel();
        AddIslandBtnDTO addIslandBtnDto = GameManager.instance.islandManager.GetAddIslandBtnDTO(currentIslandLevel);

        if (addIslandBtnDto == null)
        {
            addIslandBtn.gameObject.SetActive(false);
        }
        else
        {
            addIslandBtn.gameObject.SetActive(true);
            addIslandBtn.SetIslandBtn(addIslandBtnDto.islandType, addIslandBtnDto.islandName, ConvertGoldToText(addIslandBtnDto.unlockPrice), addIslandBtnDto.position);
        }
    }
    
    private void OnclickAddIslandBtn(IslandType islandType)
    {
        GameManager.instance.islandManager.UnlockIsland((int)islandType);
        SetAddIslandBtn();
    }

    #endregion

    #region ActionBtn

    private void InitActionBtn()
    {
        actionBtn.OnBtnUp += OnActionBtnUp;
    }

    public void OnPlayerTargetChanged(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Tree:
                actionBtn.SetBtnIcon(actionBtnIconSprites[0]);
                break;
            case ItemType.Rock:
                actionBtn.SetBtnIcon(actionBtnIconSprites[1]);
                break;
            case ItemType.Wheat:
            case ItemType.Beet:
            case ItemType.Cabbage:
            case ItemType.Carrot:
            case ItemType.Cauliflower:
            case ItemType.Kale:
            case ItemType.Parsnip:
            case ItemType.Potato:
            case ItemType.Pumpkin:
            case ItemType.Radish:
            case ItemType.Sunflower:
                actionBtn.SetBtnIcon(actionBtnIconSprites[2]);
                break;
            default:
                actionBtn.SetBtnIcon(null);
                break;
        }
    }
    
    private void OnActionBtnDown()
    {
        
    }

    private void OnActionBtnUp()
    {
        
    }

    #endregion
    
    #region FloatingText

    private void InitFloatingText()
    {
        for (int i = 0; i < floatingTextPoolSize; i++)
        {
            GameObject newFloatingText = Instantiate(floatingTextPrefab, floatingTextPool);
            floatingTexts.Add(newFloatingText.GetComponent<FloatingText>());
        }
    }

    public void ShowDamageText(int damage, GameObject target)
    {
        float produceYSize = 0;
        if (target.TryGetComponent(out Produce produce))
        {
            produceYSize = produce.TileSize.y;
        }

        for (int i = 0; i < floatingTextPoolSize; i++)
        {
            if (!floatingTexts[i].gameObject.activeSelf)
            {
                floatingTexts[i].SetTextData(target.transform.position + (Vector3.up * produceYSize / 2f), damage.ToString());
                floatingTexts[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    #endregion

    #region SpeechBubble

    public void SetPlayerSpeechBubble(PlayerState playerState)
    {
        playerSpeechBubble.SetText(playerLinesDic[playerState.ToString()]);
        playerSpeechBubble.gameObject.SetActive(true);
    }

    #endregion
}