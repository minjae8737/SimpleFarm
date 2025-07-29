using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public GameObject marker;
    public GameObject uiGaugeBar;
    public GameObject uiGauge;
    public GameObject uiHpBar;
    public GameObject uiHp;

    public float maxCoolTime;
    float coolTime;
    public float maxHp;
    float hp;
    bool isCoolTime;

    void Awake()
    {
        isCoolTime = false;
        // TODO: 나중에는 현재 쿨타임 값도 게임을 끌때 모두 저장되어야함.
        coolTime = 0f;
    }

    void Start()
    {

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
        if (isCoolTime)
            return;

        Debug.Log("OnInteract()");

        OnHpBar();
        hp -= 1; // FIXME 나중엔 플레이어 데미지 가져오기
        SetHp();

        if (hp <= 0)
        {
            OffHpBar();
            OnGauge();
            isCoolTime = true;
            coolTime = 0f;
            hp = maxHp;
        }

    }

    public void OnGauge()
    {
        uiGaugeBar.SetActive(true);
    }

    public void OffGauge()
    {
        uiGaugeBar.SetActive(false);
    }

    public void SetGauge()
    {
        uiGauge.transform.localScale = new Vector3(coolTime / maxCoolTime, 1, 1);
    }

    public void OnHpBar()
    {
        uiHpBar.SetActive(true);
    }

    public void OffHpBar()
    {
        uiHpBar.SetActive(false);
    }

    public void SetHp()
    {
        uiHp.transform.localScale = new Vector3(hp / maxHp, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
            return;

        OnInteract();
    }

}
