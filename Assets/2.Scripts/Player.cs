using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("# Stat")]
    public int maxHp;
    public int hp;
    public float speed;
    public float scanRange;
    public LayerMask targetLayer;
    Vector2 inputVec;
    
    Rigidbody2D rigid;
    Collider2D[] targets;
    GameObject nearestTarget;
    
    SpriteRenderer hairSprite;
    SpriteRenderer bodySprite;
    Animator hairAnim;
    Animator bodyAnim;
    PlayerAnimationEvent animationEvent;
    bool isActionAnim;

    public event Action<string> OnPlayerAction;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        hairSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bodySprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        hairAnim = transform.GetChild(0).GetComponent<Animator>();
        bodyAnim = transform.GetChild(1).GetComponent<Animator>();
        animationEvent = transform.GetChild(1).GetComponent<PlayerAnimationEvent>();
        
        isActionAnim = false;

        animationEvent.OnStateEnd += OnInteractionEnd;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void Init()
    {
        maxHp = GameManager.instance.GetIntFromPlayerPrefs("PalyerMaxHp") == 0 ? 10 : GameManager.instance.GetIntFromPlayerPrefs("PalyerMaxHp");
        hp = GameManager.instance.GetIntFromPlayerPrefs("PalyerHp") == 0 ? 10 : GameManager.instance.GetIntFromPlayerPrefs("PalyerHp"); ;
    }

    void SaveData()
    {
        GameManager.instance.SaveIntToPlayerPrefs("PalyerMaxHp", maxHp);
        GameManager.instance.SaveIntToPlayerPrefs("PalyerHp", hp);
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        targets = Physics2D.OverlapCircleAll(transform.position, scanRange, targetLayer);
        if (!isActionAnim)
        { 
            rigid.MovePosition(rigid.position + (inputVec * speed * Time.fixedDeltaTime));
        }

        SuchObject();
    }

    void LateUpdate()
    {
        
        hairAnim.SetFloat("Speed", inputVec.normalized.magnitude);
        bodyAnim.SetFloat("Speed", inputVec.normalized.magnitude);

        if (inputVec.x != 0)
        {
            bool isReverse = inputVec.x < 0;
            hairSprite.flipX = isReverse;
            bodySprite.flipX = isReverse;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interacting();
        }
    }

    void SuchObject()
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D target in targets)
        {
            float dis = Vector2.Distance(transform.position, target.transform.position);

            bool isObject = target.TryGetComponent<Produce>(out var component);

            if (dis < minDistance && ((isObject && !component.isCoolTime) || !isObject))
            {
                minDistance = dis;
                nearest = target.gameObject;
            }
        }

        if (nearestTarget != nearest)
        {
            if (nearestTarget != null)
            {
                nearestTarget.GetComponent<Marker>()?.OffMarker();
            }

            nearestTarget = nearest;

            if (nearestTarget != null)
            {
                nearestTarget.GetComponent<Marker>()?.OnMarker();
            }
        }
    }

    void Interacting()
    {
        if (hp <= 0)
            return;
        
        Produce produce = nearestTarget?.GetComponent<Produce>();
        
        if (isActionAnim || produce == null) return;
        
        isActionAnim = true;

        switch (produce?.type)
        {
            case ItemType.Wheat:
            case ItemType.Beet:
            case ItemType.Cabbage:
            case ItemType.Carrot:
            case ItemType.Cauliflower:
            case ItemType.Kale:
            case ItemType.Parsnip:
            case ItemType.Potato:
            case ItemType.Pumpkin:
            case ItemType.Radish:
            case ItemType.Sunflower:
                hairAnim.SetTrigger("DoDoing");
                bodyAnim.SetTrigger("DoDoing");
                break;
            case ItemType.Tree:
                hairAnim.SetTrigger("DoAxe");
                bodyAnim.SetTrigger("DoAxe");
                break;
            case ItemType.Rock:
                hairAnim.SetTrigger("DoMining");
                bodyAnim.SetTrigger("DoMining");
                break;
                // bodyAnim.SetTrigger("DoHamering");
        }
        
    }

    public void OnInteractionEnd()
    {
        if (nearestTarget != null)
        {
            nearestTarget.GetComponent<Produce>()?.OnInteract();
            LoseHp();
            OnPlayerAction?.Invoke("Interact");
        }

        isActionAnim = false;
    }

    public void LoseHp()
    {
        if (hp <= 0)
            return;

        hp -= 1;
    }

    public void RecoverHP()
    {
        if (hp == 0)
            IncreaseMaxHp();

        OnSleepEffect();
        hp = maxHp;
    }

    public void IncreaseMaxHp()
    {
        if (maxHp >= int.MaxValue)
            return;

        maxHp += 1;
    }

    public void OnSleepEffect()
    {
        gameObject.SetActive(false);
    }

    public void OffSleepEffect()
    {
        gameObject.SetActive(true);
    }

}
