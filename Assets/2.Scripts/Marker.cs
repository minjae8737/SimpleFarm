using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public GameObject marker;

    Tween scaleTween;

    void OnEnable()
    {
        OnSacleEffect();
    }

    void OnDisable()
    {
        scaleTween?.Kill();
    }

    public void OnMarker()
    {
        marker.SetActive(true);
    }

    public void OffMarker()
    {
        marker.SetActive(false);
    }

    void OnSacleEffect()
    {
        scaleTween = marker?.transform.DOScale(Vector3.one * 1.02f, 0.5f)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Yoyo);
    }

}
