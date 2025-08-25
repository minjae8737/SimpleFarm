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
    Animator anim;
    bool hasSentTrigger;
    Vector3 reverseScale = new Vector3(-1, 1, 1);


    public GameObject rake;
    public float rotDuration;
    Vector3 startRakeRot = new Vector3(0, 0, 45f);
    Vector3 endRakeRot = new Vector3(0, 0, -35f);

    public event Action<string> onPlayerAction;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hasSentTrigger = false;

        Init();
    }

    void Init()
    {
        maxHp = PlayerPrefs.HasKey("PalyerMaxHp") ? PlayerPrefs.GetInt("PalyerMaxHp") : 10;
        hp = PlayerPrefs.HasKey("PalyerHp") ? PlayerPrefs.GetInt("PalyerHp") : 10;
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("PalyerMaxHp", maxHp);
        PlayerPrefs.SetInt("PalyerHp", hp);
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        targets = Physics2D.OverlapCircleAll(transform.position, scanRange, targetLayer);
        rigid.MovePosition(rigid.position + (inputVec * speed * Time.fixedDeltaTime));

        Interacting();
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            bool isReverse = inputVec.x < 0 ? true : false;
            transform.localScale = isReverse ? reverseScale : Vector3.one;
        }
    }

    void Interacting()
    {
        if (hp <= 0)
            return;

        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D target in targets)
        {
            float dis = Vector2.Distance(transform.position, target.transform.position);

            if (dis < minDistance && !target.GetComponent<Object>().isCoolTime)
            {
                minDistance = dis;
                nearest = target.gameObject;

                if (!hasSentTrigger)
                {
                    hasSentTrigger = true;
                    OnInteractionEffect();
                }
            }
        }

        if (nearestTarget != nearest)
        {
            if (nearestTarget != null)
            {
                nearestTarget.GetComponent<Object>().OffMarker();
            }

            nearestTarget = nearest;

            if (nearestTarget != null)
            {
                nearestTarget.GetComponent<Object>().OnMarker();
            }
        }
    }

    void OnInteractionEffect()
    {
        Sequence sequence = DOTween.Sequence();

        rake.transform.localRotation = Quaternion.Euler(startRakeRot);
        Tween rotateTween = rake.transform.DOLocalRotate(endRakeRot, rotDuration).SetEase(Ease.InOutExpo);

        sequence.Append(rotateTween)
        .OnComplete(OnInteractionEnd);
    }

    public void OnInteractionEnd()
    {

        if (nearestTarget != null)
        {
            nearestTarget.GetComponent<Object>().OnInteract();
            LoseHp();
            onPlayerAction?.Invoke("Interact");
        }

        rake.transform.localRotation = Quaternion.identity;
        hasSentTrigger = false;
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
