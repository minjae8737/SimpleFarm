using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public GameObject marker;

    private Transform marker_bl;
    private Transform marker_br;
    private Transform marker_tr;
    private Transform marker_tl;
    
    private Vector3 originalPos_bl;
    private Vector3 originalPos_br;
    private Vector3 originalPos_tr;
    private Vector3 originalPos_tl;
    
    private Tween tween_bl;
    private Tween tween_br;
    private Tween tween_tr;
    private Tween tween_tl;

    private void Awake()
    {
        marker_bl = marker.transform.GetChild(0);
        marker_br = marker.transform.GetChild(1);
        marker_tr = marker.transform.GetChild(2);
        marker_tl = marker.transform.GetChild(3);
        
        originalPos_bl = marker_bl.position;
        originalPos_br = marker_br.position;
        originalPos_tr = marker_tr.position;
        originalPos_tl = marker_tl.position;
    }

    private void OnEnable()
    {
        ResetPositions();
        OnSacleEffect();
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

    public void OnMarker()
    {
        marker.SetActive(true);
    }

    public void OffMarker()
    {
        marker.SetActive(false);
    }
    
    private void ResetPositions()
    {
        if (marker_bl != null) marker_bl.position = originalPos_bl;
        if (marker_br != null) marker_br.position = originalPos_br;
        if (marker_tr != null) marker_tr.position = originalPos_tr;
        if (marker_tl != null) marker_tl.position = originalPos_tl;
    }

    private void OnSacleEffect()
    {
        KillAllTweens();
        
        tween_bl = marker_bl?.DOMove(marker_bl.position + (Vector3.left + Vector3.down) * 0.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        tween_br = marker_br?.DOMove(marker_br.position + (Vector3.right + Vector3.down) * 0.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        tween_tr = marker_tr?.DOMove(marker_tr.position + (Vector3.right + Vector3.up) * 0.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        tween_tl = marker_tl?.DOMove(marker_tl.position + (Vector3.left + Vector3.up) * 0.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    
    private void KillAllTweens()
    {
        tween_bl?.Kill();
        tween_br?.Kill();
        tween_tr?.Kill();
        tween_tl?.Kill();
        
        tween_bl = null;
        tween_br = null;
        tween_tr = null;
        tween_tl = null;
    }
}