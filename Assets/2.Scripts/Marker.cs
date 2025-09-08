using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public GameObject marker;
    
    public Transform marker_bl;
    public Transform marker_br;
    public Transform marker_tr;
    public Transform marker_tl;
    Tween scaleTween;


    private void Awake()
    {
        marker_bl = marker.transform.GetChild(0);
        marker_br = marker.transform.GetChild(1);
        marker_tr = marker.transform.GetChild(2);
        marker_tl = marker.transform.GetChild(3);
    }

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
        marker_bl?.DOMove(marker_bl.position + (Vector3.left + Vector3.down) * 0.1f,0.5f).SetLoops(-1, LoopType.Yoyo); 
        marker_br?.DOMove(marker_br.position + (Vector3.right + Vector3.down) * 0.1f,0.5f).SetLoops(-1, LoopType.Yoyo);
        marker_tr?.DOMove(marker_tr.position + (Vector3.right + Vector3.up) * 0.1f,0.5f).SetLoops(-1, LoopType.Yoyo);
        marker_tl?.DOMove(marker_tl.position + (Vector3.left + Vector3.up) * 0.1f,0.5f).SetLoops(-1, LoopType.Yoyo);
        
        // scaleTween = marker?.transform.DOScale(Vector3.one * 1.05f, 0.5f)
        // .SetLoops(-1, LoopType.Yoyo);
    }

}
