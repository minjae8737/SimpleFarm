using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddIslandBtn : MonoBehaviour
{
    [SerializeField] private IslandType addIslandType;
    [SerializeField] private TextMeshProUGUI islandNameText;
    [SerializeField] private TextMeshProUGUI islandPriceText;
    public event Action<IslandType> OnClick;

    public void SetIslandBtn(IslandType islandType, string islandName, string islandPrice)
    {
        addIslandType =  islandType;
        islandNameText.text = islandName;
        islandPriceText.text = islandPrice;
    }

    public void OnClickBtn()
    {
        transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f, 1, 1f);
        OnClick?.Invoke(addIslandType);
    }
}