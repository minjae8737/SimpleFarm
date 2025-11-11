using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private Image iconImg;
    [SerializeField] private RectTransform iconTransform;
    
    public event Action OnBtnDown;
    public event Action OnBtnUp;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        iconTransform.anchoredPosition = Vector2.down;
        OnBtnDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        iconTransform.anchoredPosition = Vector2.zero;
        OnBtnUp?.Invoke();
    }

    public void SetBtnIcon(Sprite icon)
    {
        iconImg.enabled = icon != null;
        iconImg.sprite = icon;
        iconImg.SetNativeSize();
    }
}