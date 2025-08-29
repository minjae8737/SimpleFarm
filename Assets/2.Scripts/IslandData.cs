using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Island_", menuName = "ScriptableObject/Island Data")]
public class IslandData : ScriptableObject
{
    public enum IslandType { Basic, Wheat, Apple }

    [Header("# Info")]
    public IslandType islandType;
    public string name;
    public FarmData farmData;

}
