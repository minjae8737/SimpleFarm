using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum UIBtnType { RecoverHP, Shop, }

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
    
    [Header("Shop")]
    public GameObject shopPanel;
    RectTransform shopContent;
    public GameObject shopItemPrefab;
    List<GameObject>  shopItems;
    public ItemSellPanel itemSellPanel;

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

        SetPlayerHp();
        SetGoldText();
        SetQuestPanel();
        InitShopItems();
        
        GameManager.instance.player.OnPlayerAction += SetPlayerHp;
        GameManager.instance.questManager.OnQuestProgressChanged += SetQuestPanel;

        GameManager.instance.inventory.OnItemAdded += RefreshShopItem;
        GameManager.instance.inventory.OnItemRemoved += RefreshShopItem;
    }

    #region Gold
    
    public void SetGoldText()
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

        goldD = Math.Floor(goldD * 100) / 100;  // 반올림 방지
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

        string questDescStr = string.Format("{0} [{1}/{2}]", questData.desc, GameManager.instance.questManager.curCount, questData.targetCount);
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
        shopItems = new  List<GameObject>();
        
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            GameObject newShopItem = Instantiate(shopItemPrefab, shopContent);
            shopItems.Add(newShopItem);
            newShopItem.GetComponent<ShopItem>().Init(GameManager.instance.itemDatas[(int)type]);
        }
    }

    void RefreshShopItem(ItemData itemData, long quantity)
    {
        GameObject findShopItem = shopItems.Find(item => item.GetComponent<ShopItem>().itemName == itemData.itemName);
        long itemQuantity = GameManager.instance.inventory.GetItemQuantity(itemData.itemName);
        findShopItem?.GetComponent<ShopItem>().RefreshQuantity(itemQuantity);
    }

    #endregion
    
    #region ItemSellPanel

    void SetItemSellPanel()
    {
        itemSellPanel.Init(GameManager.instance.GetItemData((int)ItemType.Wheat),this);
    }

    #endregion
}
