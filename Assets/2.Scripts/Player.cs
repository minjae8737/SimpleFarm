using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PlayerState
{
    LowHp,
    
}

public class Player : MonoBehaviour
{
    [Header("# Stat")]
    [SerializeField] private PlayerStrength playerStrength;
    public PlayerStrength PlayerStrength => playerStrength;
    [SerializeField] private int maxHp;
    public int MaxHp => maxHp;
    [SerializeField] private int hp;
    public int Hp => hp;
    [SerializeField] private float speed;
    [SerializeField] private float scanRange;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private DynamicJoystick dynamicJoystick;
    private Vector2 inputVec;
    
    private Rigidbody2D rigid;
    private Collider2D[] targets;
    private int targetCount = 0;
    private GameObject nearestTarget;
    
    private SpriteRenderer hairSprite;
    private SpriteRenderer bodySprite;
    private Animator hairAnim;
    private Animator bodyAnim;
    private PlayerAnimationEvent animationEvent;
    private bool isActionAnim;

    public event Action<string> OnPlayerAction;
    public event Action<int, GameObject> OnStrengthUsed; 
    public event Action<ItemType> OnTargetChanged;
    public event Action<PlayerState> OnPlayerStateUpdated;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        hairSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bodySprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        hairAnim = transform.GetChild(0).GetComponent<Animator>();
        bodyAnim = transform.GetChild(1).GetComponent<Animator>();
        animationEvent = transform.GetChild(1).GetComponent<PlayerAnimationEvent>();
        
        isActionAnim = false;
        targets =  new Collider2D[30];

        animationEvent.OnStateEnd += OnActionEnd;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void Init()
    {
        maxHp = GameManager.instance.GetIntFromPlayerPrefs("PalyerMaxHp", 10);
        hp = GameManager.instance.GetIntFromPlayerPrefs("PalyerHp", 10);
        playerStrength.SetStrength(GameManager.instance.GetIntFromPlayerPrefs("PalyerStrength", 1));
    }

    void SaveData()
    {
        GameManager.instance.SaveIntToPlayerPrefs("PalyerMaxHp", maxHp);
        GameManager.instance.SaveIntToPlayerPrefs("PalyerHp", hp);
        GameManager.instance.SaveIntToPlayerPrefs("PalyerStrength", playerStrength.Strength);
    }

    void Update()
    {
        // inputVec.x = Input.GetAxisRaw("Horizontal");
        // inputVec.y = Input.GetAxisRaw("Vertical");
        inputVec.x = dynamicJoystick.Horizontal;
        inputVec.y = dynamicJoystick.Vertical;
    }

    void FixedUpdate()
    {
        targetCount = Physics2D.OverlapCircleNonAlloc(transform.position, scanRange, targets, targetLayer);
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
            DoAction();
        }
    }

    private void SuchObject()
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < targetCount; i++)
        {
            Collider2D target = targets[i];
            float dis = Vector2.Distance(transform.position, target.transform.position);

            bool isObject = target.TryGetComponent<Produce>(out var produce);
            
            if (dis < minDistance && ((isObject && !produce.isCoolTime) || !isObject))
            {
                minDistance = dis;
                nearest = target.gameObject;
            }
        }
        
        // foreach (Collider2D target in targets)
        // {
        //     float dis = Vector2.Distance(transform.position, target.transform.position);
        //
        //     bool isObject = target.TryGetComponent<Produce>(out var produce);
        //     
        //     if (dis < minDistance && ((isObject && !produce.isCoolTime) || !isObject))
        //     {
        //         minDistance = dis;
        //         nearest = target.gameObject;
        //     }
        //     
        // }
        
        if (nearestTarget != nearest)
        {
            if ((object)nearestTarget != null)
            {
                nearestTarget.GetComponent<Marker>()?.OffMarker();
            }

            nearestTarget = nearest;

            if ((object)nearestTarget != null && nearestTarget.TryGetComponent(out Produce produce))
            {
                OnTargetChanged?.Invoke(produce.Type);
            }
            else
            {
                OnTargetChanged?.Invoke(ItemType.None);
            }

            if ((object)nearestTarget != null)
            {
                nearestTarget.GetComponent<Marker>()?.OnMarker();
            }
        }
    }

    public void DoAction()
    {
        if (hp <= 0)
        {
            OnPlayerStateUpdated?.Invoke(PlayerState.LowHp);
            return;
        }
        
        Produce produce = nearestTarget?.GetComponent<Produce>();
        
        if (isActionAnim || produce == null) return;
        
        isActionAnim = true;

        switch (produce?.Type)
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

    public void OnActionEnd()
    {
        if (nearestTarget != null)
        {
            nearestTarget.GetComponent<Produce>()?.OnInteract(playerStrength.Strength);
            LoseHp();
            OnPlayerAction?.Invoke("Interact");
            OnStrengthUsed?.Invoke(playerStrength.Strength, nearestTarget);
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
