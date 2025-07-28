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


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
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
    }

    void LateUpdate()
    {
        Interacting();
    }

    void Interacting()
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D target in targets)
        {
            float dis = Vector2.Distance(transform.position, target.transform.position);

            if (dis < minDistance)
            {
                minDistance = dis;
                nearest = target.gameObject;
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
}
