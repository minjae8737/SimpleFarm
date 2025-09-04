using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IslandType { Basic, Wheat, Apple }

[CreateAssetMenu(fileName = "Island_", menuName = "ScriptableObject/Island Data")]
public class IslandData : ScriptableObject
{

    [Header("# Info")]
    public IslandType islandType;
    public string name;
    public FarmData farmData;

}
