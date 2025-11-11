using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    
    private Sequence sequence;


    private void OnEnable()
    {
        RefreshSize();
        Effect();
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

    private void Effect()
    {
        sequence?.Kill();
        
        transform.localScale = Vector3.zero;
        text.gameObject.SetActive(false);

        sequence = DOTween.Sequence();
        
        // 커지는 효과
        sequence.Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
            .AppendCallback(() => text.gameObject.SetActive(true));
        sequence.AppendInterval(3f);
        sequence.OnComplete(() => gameObject.SetActive(false));
    }
}
