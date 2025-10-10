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
}

public class UIManager : MonoBehaviour
{
    [Header("HUD")] 
    public Text goldText;
    public GameObject questPanel;
    Image questRewardIcon;
    Text questRewardText;
    Text questDesc;
    GameObject questClearBtn;
    Image questClearBtnRewardIcon;
    Text questClearBtnRewardText;
    Text questClearBtnDesc;
    public GameObject playerHp;
    Image palyerHpImg;
    Text playerHpText;
    float recoveHpDuration = 3f; // FillAmount 0 -> 1 까지 걸리는 시간
    public Button interactButton;

    [Header("Menu")] 
    public Button inventoryBtn;
    public Button magnetBtn;
    private TextMeshProUGUI magnetBtnText;

    [Header("Shop")] 
    public GameObject shopPanel;
    RectTransform shopContent;
    public GameObject shopItemPrefab;
    List<GameObject> shopItems;
    public ItemSellPanel itemSellPanel;

    [Header("Item")] 
    public GameObject itemInfoPanel;
    Text itemNameText;
    Image itemIcon;
    Text itemPriceText;
    Text itemQuantityText;
    Text itemDescriptionText;

    [Header("Farm")]
    public FarmUpgradePanel farmUpgradePanel;
    
    [Header("Inventory")]
    public InventoryPanel inventoryPanel;
    
    [Header("AddIslandBtn")]
    public AddIslandBtn addIslandBtn;
    
    [Header("Sprites")] 
    public Sprite[] rewardIcons; // RewardsType과 매칭

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

        // 상점
        shopContent = shopPanel.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();

        // 아이템
        itemNameText = itemInfoPanel.transform.GetChild(0).GetComponent<Text>();
        itemIcon = itemInfoPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        itemPriceText = itemInfoPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        itemQuantityText = itemInfoPanel.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        itemDescriptionText = itemInfoPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        
        // 메뉴 버튼
        magnetBtnText =  magnetBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        SetPlayerHp();
        RefreshGoldText();
        SetQuestPanel();
        InitShopItems();
        InitInventoryPanel();
        InitAddIslandBtn();
        
        GameManager.instance.player.OnPlayerAction += SetPlayerHp;
        GameManager.instance.questManager.OnQuestProgressChanged += SetQuestPanel;

        GameManager.instance.inventory.OnItemAdded += RefreshShopItem;
        GameManager.instance.inventory.OnItemRemoved += RefreshShopItem;
        GameManager.instance.inventory.OnItemAdded += RefreshInventoryPanel;
        GameManager.instance.inventory.OnItemRemoved += RefreshInventoryPanel;

        itemSellPanel.OffItemInfoPanel += OffItemInfoPanel;
        
        farmUpgradePanel.OnClickFarmUpgradeBtn += OnClickFarmUpgradeBtn;
        farmUpgradePanel.OnClickAutoUpgradeBtn += OnClickAutoUpgradeBtn;
        farmUpgradePanel.OnClickCooldownUpgradeBtn += OnClickCooldownUpgradeBtn;
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

    void SetPlayerHp(string defaultPlayerAction = "")
    {
        float playerHpAmount = (float)GameManager.instance.player.hp / GameManager.instance.player.maxHp;
        palyerHpImg.fillAmount = playerHpAmount;

        SetPlayerHpText();
    }

    void SetPlayerHpText()
    {
        playerHpText.text = GameManager.instance.player.hp + " / " + GameManager.instance.player.maxHp;
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
        QuestData questData = GameManager.instance.questManager.GetCurrentQuestData();

        if (questData == null)
        {
            questPanel.SetActive(false);
            return;
        }

        string questDescStr = string.Format("{0} [{1}/{2}]", questData.desc, GameManager.instance.questManager.curCount,
            questData.targetCount);
        string questRewardAmountStr = ConvertGoldToText(questData.rewardAmount);


        questDesc.text = questDescStr;
        questRewardIcon.sprite = rewardIcons[(int)questData.rewardsType];
        questRewardText.text = questRewardAmountStr;

        questClearBtnDesc.text = questDescStr;
        questClearBtnRewardIcon.sprite = rewardIcons[(int)questData.rewardsType];
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
        interactButton.onClick.RemoveAllListeners();

        switch (type)
        {
            case UIBtnType.RecoverHP:
                interactButton.onClick.AddListener(GameManager.instance.RestPlayer);
                break;
            case UIBtnType.Shop:
                interactButton.onClick.AddListener(OpenShop);
                break;
            case UIBtnType.Island_Wheat:
                interactButton.onClick.AddListener(() => SetFarmUpgradePanel(IslandType.Wheat));
                break;
        }

        OnInteractBtnEffect();
    }

    void OnInteractBtnEffect()
    {
        if (interactButton == null)
            return;

        interactButton.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        Tween scaleBiggerTween = interactButton.GetComponent<RectTransform>().DOScale(1.02f, 0.2f);
        Tween scaleSmallerTween = interactButton.GetComponent<RectTransform>().DOScale(1f, 0.2f);

        sequence.Append(scaleBiggerTween)
            .Append(scaleSmallerTween);
    }

    public void OffInteractBtn()
    {
        if (interactButton == null)
            return;

        interactButton.gameObject.SetActive(false);
    }

    void OnClickInteractBtn()
    {
        interactButton.onClick?.Invoke();
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
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        shopPanel.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
    }

    #region Shop

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void InitShopItems()
    {
        shopItems = new List<GameObject>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            GameObject newShopItem = Instantiate(shopItemPrefab, shopContent);
            shopItems.Add(newShopItem);
            ShopItem shopItem = newShopItem.GetComponent<ShopItem>();
            shopItem.Init(GameManager.instance.itemDatas[(int)type]);
            shopItem.OnClickShopItem += OnItemSellPanel;
        }
    }

    void RefreshShopItem(ItemData itemData, long quantity)
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
    }

    void OffItemInfoPanel()
    {
        itemInfoPanel.SetActive(false);
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

    private void SetFarmUpgradePanel(IslandType islandType)
    {
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
        farmUpgradePanel.gameObject.SetActive(true);
    }

    private void OnClickFarmUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold)) return;
        
        GameManager.instance.islandManager.LevelUpFarm((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
    }

    private void OnClickAutoUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold)) return;
        
        GameManager.instance.islandManager.LevelUpAutoProduceChance((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
    }

    private void OnClickCooldownUpgradeBtn(IslandType islandType, long gold)
    {
        if (!GameManager.instance.CheckGold(gold)) return;
        
        GameManager.instance.islandManager.LevelUpProduceCooldown((int)islandType);
        GameManager.instance.SetGold(-gold);
        farmUpgradePanel.RefreshPanel(GameManager.instance.islandManager.GetFarmUpgradePanelDTO((int)islandType));
    }
    
    #endregion

    #region InventoryPanel

    void InitInventoryPanel()
    {
        inventoryPanel.Init();
    }
    
    void RefreshInventoryPanel(ItemData itemData, long quantity)
    {
        inventoryPanel.RefreshInventoryPanel(itemData);
    }

    public void OpenInventoryPanel()
    {
        questPanel.SetActive(false);
        playerHp.SetActive(false);
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
    }

    public void OffInventoryPanel()
    {
        inventoryPanel.gameObject.SetActive(false);
        questPanel.SetActive(true);
        playerHp.SetActive(true);
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
        Debug.Log(currentIslandLevel);
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
}