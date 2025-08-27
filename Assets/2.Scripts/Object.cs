using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { None, Wheat, Apple }

public class Object : MonoBehaviour
{
    public ObjectType type;
    public GameObject uiGaugeBar;
    public GameObject uiGauge;
    public GameObject uiHpBar;
    public GameObject uiHp;

    float maxCoolTime;
    float coolTime;
    float maxHp;
    float hp;
    public bool isCoolTime;
    public ObjectData data;

    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        Init();
    }

    void Init()
    {
        isCoolTime = false;
        maxCoolTime = data.coolTime;
        // TODO: 나중에는 현재 쿨타임 값도 게임을 끌때 모두 저장되어야함.
        coolTime = 0f;

        maxHp = data.maxHp;
        hp = maxHp;

        SetSprite();
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
            SetSprite();
        }
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
            Item dropItem = GameManager.instance.GetDropItem(type).GetComponent<Item>();
            dropItem.SetItemPos(transform.position);
            dropItem.DropItem();

            OffHpBar();
            OnGauge();
            isCoolTime = true;
            coolTime = 0f;
            hp = maxHp;
            SetSprite();
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

    public void SetSprite()
    {
        sprite.sprite = isCoolTime ? data.sprites[0] : data.sprites[1];
    }
}
