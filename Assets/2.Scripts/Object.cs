using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public GameObject marker;
    public GameObject gaugeBar;
    public GameObject gauge;

    public float maxCoolTime;
    float coolTime;
    bool isCoolTime;

    void Awake()
    {
        isCoolTime = false;
        // TODO: 나중에는 현재 쿨타임 값도 게임을 끌때 모두 저장되어야함.
        coolTime = 0f;
    }

    void Start()
    {
        OnGauge();
        OnInteract();
    }

    void Update()
    {
        if (!isCoolTime)
            return;

        coolTime += Time.deltaTime;
        SetGauge();

        if (maxCoolTime <= coolTime)
        {
            isCoolTime = false;
            OffGauge();
        }
    }

    public void OnMarker()
    {
        marker.SetActive(true);
    }

    public void OffMarker()
    {
        marker.SetActive(false);
    }

    public void OnInteract()
    {
        isCoolTime = true;
        coolTime = 0f;

    }

    public void OnGauge()
    {
        gaugeBar.SetActive(true);
    }

    public void OffGauge()
    {
        gaugeBar.SetActive(false);
    }

    public void SetGauge()
    {
        gauge.transform.localScale = new Vector3(coolTime / maxCoolTime, 1, 1);
    }


}
