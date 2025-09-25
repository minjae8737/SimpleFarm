using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmUpgradePanelDTO
{
    public IslandType islandType;   
    public string farmTitle;
    public int maxFarmLevel;
    public int farmLevel;
    public float farmCurrentValue;
    public float farmNextValue;
    public long farmGold;
    
    public int autoLevel;
    public int maxAutoLevel;
    public float autoCurrentValue;
    public float autoNextValue;
    public long autoGold;
    
    public int cooldownLevel;
    public int maxCooldownLevel;
    public float cooldownCurrentValue;
    public float cooldownNextValue;
    public long cooldownGold;
}