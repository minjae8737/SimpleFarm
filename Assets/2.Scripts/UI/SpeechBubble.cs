using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 positionOffset;

    [Header("Components")] 
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Padding")]
    [SerializeField] private Vector2 padding;
    
    private void Awake()
    {
        backgroundRect = transform.GetChild(0).GetComponent<RectTransform>();
        text =  transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        RefreshSize();
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + positionOffset;    
    }
    
    public void SetText(string message)
    {
        text.text = message;
        RefreshSize();
    }

    private void RefreshSize()
    {
        // 텍스트 렌더링 업데이트 강제
        text.ForceMeshUpdate();

        // 텍스트 영역 크기 계산
        Vector2 textSize = text.GetRenderedValues(false);

        // 패딩 포함한 배경 사이즈 설정
        backgroundRect.sizeDelta = textSize + padding;

        text.rectTransform.anchoredPosition = (backgroundRect.sizeDelta - Vector2.one) / 2f;
    }
}
