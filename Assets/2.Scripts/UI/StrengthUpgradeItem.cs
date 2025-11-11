using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StrengthUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetDictionaryItem(Sprite icon, string text)
    {
        iconImage.sprite = icon;
        countText.text = text;
    }
}
