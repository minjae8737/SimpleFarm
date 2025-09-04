using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AddIslandBtn : MonoBehaviour
{
    public IslandType addIslandType;
    public Text islandNameText;
    public Text islandPriceText;
    public event Action<IslandType> OnClick;

    public void SetIslandBtn(string islandName, string islandPrice)
    {
        islandNameText.text = islandName;
        islandPriceText.text = islandPrice;
    }
    
    public void OnClickBtn()
    {
        transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
        Debug.Log("OnMouseDown");
    }
}
