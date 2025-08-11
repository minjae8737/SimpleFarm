using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object_", menuName = "ScriptbleObject/Object Data")]
public class ObjectData : ScriptableObject
{

    [Header("# Info")]
    public float maxHp;
    public float coolTime;
    public Sprite[] sprites;

}
