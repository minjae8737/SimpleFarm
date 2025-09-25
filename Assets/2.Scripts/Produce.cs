using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Produce : MonoBehaviour
{
    [Header("# Produce Info")] 
    public ItemType type;
    public ProductData productData;

    [Header("# Gauge UI")] 
    public GameObject uiGaugeBar;
    public GameObject uiGauge;
    public GameObject uiHpBar;
    public GameObject uiHp;

    float autoProduceChance; // (%)
    float maxCoolTime;  // (sec)
    float coolTime;
    float maxHp;
    float hp;

    public bool isCoolTime;
    
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        Init();
    }

    void Init()
    {
        isCoolTime = false;
        maxCoolTime = productData.coolTime;
        autoProduceChance = 0;
        // TODO: 나중에는 현재 쿨타임 값도 게임을 끌때 모두 저장되어야함.
        coolTime = 0f;

        maxHp = productData.maxHp;
        hp = maxHp;

        SetSprite();
    }

    public void SetDatas(float produceChance, float produceCooldown)
    {
        autoProduceChance = produceChance;
        maxCoolTime  = produceCooldown;
    }

    public void SetAutoProduce(float produceChance)
    {
        autoProduceChance = produceChance;
    }

    public void SetCoolTime(float produceCooldown)
    {
        maxCoolTime = produceCooldown;
    }

    void Update()
    {
        if (!isCoolTime)
            return;

        coolTime += Time.deltaTime;
        SetGauge();
        SetSprite();

        if (maxCoolTime <= coolTime)
        {
            isCoolTime = false;
            OffGauge();
            TryAutoProduce();
        }
    }

    public void OnInteract()
    {
        if (isCoolTime)
            return;
        
        OnHpBar();
        hp -= 1; // FIXME 나중엔 플레이어 데미지 가져오기
        SetHp();

        if (hp <= 0)
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        DroppedItem droppedItem = GameManager.instance.GetDropItem(type).GetComponent<DroppedItem>();
        droppedItem.SetItemPos(transform.position);
        droppedItem.DropItem();

        OffHpBar();
        OnGauge();
        isCoolTime = true;
        coolTime = 0f;
        hp = maxHp;
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
        int idx = (int)(uiGauge.transform.localScale.x * (productData.sprites.Length - 1));
        sprite.sprite = productData.sprites[idx];
    }

    private void TryAutoProduce()
    {
        if (Random.Range(0f, 100f) < autoProduceChance) 
            DropItem();
    }
}