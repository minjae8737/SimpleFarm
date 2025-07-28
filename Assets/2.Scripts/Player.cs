using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Vector2 inputVec;
    public float speed;
    public float scanRange;
    public LayerMask targetLayer;
    Rigidbody2D rigid;


    public Collider2D[] targets;
    public GameObject nearestTarget;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

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
