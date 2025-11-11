using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Product_", menuName = "ScriptableObject/Product")]
public class ProductData : ScriptableObject
{
    [Header("# Info")]
    public string productName;
    public float maxHp;
    public float coolTime;
    public Sprite[] sprites;
    
    [Header("# Product")]
    public ItemData[] productItems; // 드랍 아이템
    public int[] productQuantities; // 각 아이템의 수량
}
