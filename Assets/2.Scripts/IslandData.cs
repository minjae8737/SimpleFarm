using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IslandType {     
    Wheat,
    Beet,
    Cabbage,
    Carrot,
    Cauliflower,
    Kale,
    Parsnip,
    Potato,
    Pumpkin,
    Radish,
    Sunflower, 
}

[CreateAssetMenu(fileName = "Island_", menuName = "ScriptableObject/Island Data")]
public class IslandData : ScriptableObject
{

    [Header("# Info")]
    public IslandType islandType;
    public string name;
    public FarmData farmData;

}
