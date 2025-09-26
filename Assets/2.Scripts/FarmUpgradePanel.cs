using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmUpgradePanel : MonoBehaviour
{
    private IslandType islandType;
    [SerializeField] private TextMeshProUGUI farmNameText;
    [SerializeField] private Button closeButton;

    [Header("Farm Level")]
    [SerializeField] private TextMeshProUGUI farmTitleText;
    [SerializeField] private TextMeshProUGUI farmLevelText;
    [SerializeField] private TextMeshProUGUI farmCurrentText;
    [SerializeField] private TextMeshProUGUI farmNextText;
    [SerializeField] private Button farmUpgradeBtn;
    [SerializeField] private TextMeshProUGUI farmBtnLevelText;
    [SerializeField] private TextMeshProUGUI farmBtnGoldText;
    private long farmGold;
    
    [Header("Auto Produce")]
    [SerializeField] private TextMeshProUGUI autoLevelText;
    [SerializeField] private TextMeshProUGUI autoCurrentText;
    [SerializeField] private TextMeshProUGUI autoNextText;
    [SerializeField] private Button autoUpgradeBtn;
    [SerializeField] private TextMeshProUGUI autoBtnLevelText;
    [SerializeField] private TextMeshProUGUI autoBtnGoldText;
    private long autoGold;
    
    [Header("Cool Down")]
    [SerializeField] private TextMeshProUGUI cooldownLevelText;
    [SerializeField] private TextMeshProUGUI cooldownCurrentText;
    [SerializeField] private TextMeshProUGUI cooldownNextText;
    [SerializeField] private Button cooldownUpgradeBtn;
    [SerializeField] private TextMeshProUGUI cooldownBtnLevelText;
    [SerializeField] private TextMeshProUGUI cooldownBtnGoldText;
    private long cooldownGold;
    
    [Header("Resource")]
    [SerializeField] private Sprite[] btnSprites;
    
    public event Action<IslandType, long> OnClickFarmUpgradeBtn;
    public event Action<IslandType, long> OnClickAutoUpgradeBtn;
    public event Action<IslandType, long> OnClickCooldownUpgradeBtn;

    private void Awake()
    {
        farmUpgradeBtn.onClick.AddListener(() => OnClickFarmUpgradeBtn?.Invoke(islandType, farmGold));
        autoUpgradeBtn.onClick.AddListener(() => OnClickAutoUpgradeBtn?.Invoke(islandType, autoGold));
        cooldownUpgradeBtn.onClick.AddListener(() => OnClickCooldownUpgradeBtn?.Invoke(islandType, cooldownGold));
    }

    public void RefreshPanel(FarmUpgradePanelDTO farmUpgradePanelDTO)
    {
        islandType =  farmUpgradePanelDTO.islandType;
        farmNameText.text = farmUpgradePanelDTO.farmTitle;

        bool canUpgradeFarmLevel = farmUpgradePanelDTO.farmLevel < farmUpgradePanelDTO.maxFarmLevel;
        farmTitleText.text = farmUpgradePanelDTO.farmTitle.Split(" ")[0] + " 구매"; // "사과 나무 구매"
        farmLevelText.text = "Lv." + farmUpgradePanelDTO.farmLevel;
        farmCurrentText.text = farmUpgradePanelDTO.farmCurrentValue.ToString();
        farmNextText.text = farmUpgradePanelDTO.farmNextValue < farmUpgradePanelDTO.maxFarmLevel ? farmUpgradePanelDTO.farmNextValue.ToString() : "Max";
        farmBtnLevelText.text = farmUpgradePanelDTO.farmLevel + 1 <= farmUpgradePanelDTO.maxFarmLevel ? "Lv." + (farmUpgradePanelDTO.farmLevel + 1) : "Max";
        farmGold =  farmUpgradePanelDTO.farmGold;
        farmBtnGoldText.text = GameManager.instance.uiManager.ConvertGoldToText(farmGold);

        farmUpgradeBtn.enabled = canUpgradeFarmLevel;
        farmUpgradeBtn.gameObject.GetComponent<Image>().sprite = canUpgradeFarmLevel ? btnSprites[0] : btnSprites[1]; 
        
        bool canUpgradeAutoLevel = farmUpgradePanelDTO.autoLevel < farmUpgradePanelDTO.maxAutoLevel;
        autoLevelText.text = "Lv." + farmUpgradePanelDTO.autoLevel;
        autoCurrentText.text = farmUpgradePanelDTO.autoCurrentValue + "%";
        autoNextText.text = farmUpgradePanelDTO.autoLevel < farmUpgradePanelDTO.maxAutoLevel ? farmUpgradePanelDTO.autoNextValue + "%" : "Max";
        autoBtnLevelText.text = farmUpgradePanelDTO.autoLevel + 1 <= farmUpgradePanelDTO.maxAutoLevel ? "Lv." + (farmUpgradePanelDTO.autoLevel + 1) : "Max";
        autoGold  =  farmUpgradePanelDTO.autoGold;
        autoBtnGoldText.text = GameManager.instance.uiManager.ConvertGoldToText(autoGold);

        autoUpgradeBtn.enabled = canUpgradeAutoLevel;
        autoUpgradeBtn.gameObject.GetComponent<Image>().sprite = canUpgradeAutoLevel ? btnSprites[0] : btnSprites[1];

        bool canUpgradeCooldownLevel = farmUpgradePanelDTO.cooldownLevel < farmUpgradePanelDTO.maxCooldownLevel;
        cooldownLevelText.text = "Lv." + farmUpgradePanelDTO.cooldownLevel;
        cooldownCurrentText.text = farmUpgradePanelDTO.cooldownCurrentValue + "s";
        cooldownNextText.text = farmUpgradePanelDTO.cooldownLevel < farmUpgradePanelDTO.maxCooldownLevel ? farmUpgradePanelDTO.cooldownNextValue + "s" : "Max";
        cooldownBtnLevelText.text = farmUpgradePanelDTO.cooldownLevel + 1 <= farmUpgradePanelDTO.maxCooldownLevel ? "Lv." + (farmUpgradePanelDTO.cooldownLevel + 1) : "Max";
        cooldownGold = farmUpgradePanelDTO.cooldownGold;
        cooldownBtnGoldText.text = GameManager.instance.uiManager.ConvertGoldToText(cooldownGold);
        
        cooldownUpgradeBtn.enabled = canUpgradeCooldownLevel;
        cooldownUpgradeBtn.gameObject.GetComponent<Image>().sprite = canUpgradeCooldownLevel ? btnSprites[0] : btnSprites[1]; 

    }
    
    
    
}
