using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floatingText;
    private Vector3 startPosition;
    
    [Header("Tween Info")]
    [SerializeField] private float tweenEndValue;
    [SerializeField] private float duration;

    private void OnEnable()
    {
        TextEffect();
    }

    public void SetTextData(Vector3 position, string text)
    {
        transform.position = position;
        floatingText.text = text;
    }

    private void TextEffect()
    {
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(transform.DOMoveY(transform.position.y + tweenEndValue, duration).SetEase(Ease.Unset))
            .OnComplete(OnCompleteTextEffect);
    }
    
    private void OnCompleteTextEffect()
    {
        gameObject.SetActive(false);
    }
}
