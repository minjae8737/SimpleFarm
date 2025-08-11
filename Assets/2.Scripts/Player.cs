using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

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

    public event Action<string> onPlayerAction;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hasSentTrigger = false;
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
                    anim.SetTrigger("Interact");
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

    public void OnInteractionEnd()
    {
        if (nearestTarget != null)
        {
            nearestTarget.GetComponent<Object>().OnInteract();
            onPlayerAction?.Invoke("Interact");
        }

        hasSentTrigger = false;
    }
}
