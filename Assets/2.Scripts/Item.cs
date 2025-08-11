using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{

    float arcHeight = 0.2f; // 포물선의 높이

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropItem();
        }
    }

    // 아이템 떨어지는 효과
    public void DropItem()
    {

        Vector3 myPos = transform.position;
        Vector3 ranPos = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.5f, -0.3f), 0);
        Vector3 endPos = myPos + ranPos;

        Vector3 midPoint = (myPos + endPos) / 2;
        midPoint.y += arcHeight;

        Vector3[] path = { myPos, midPoint, endPos };
        transform.DOPath(path, 0.5f, PathType.CatmullRom);

    }
}
