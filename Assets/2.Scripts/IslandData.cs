using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Island_", menuName = "ScriptbleObject/Island Data")]
public class IslandData : ScriptableObject
{
    [Header("# Info")]
    public string name;
    public FarmData farmData;

}
