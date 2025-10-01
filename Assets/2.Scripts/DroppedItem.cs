using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("Item Info")]
    public ItemData itemData;

    [Header("Animation Setting")]
    float arcHeight = 0.2f; // 포물선의 높이
    float pickupSpeed = 2.5f; // 0.5의 거리를 0.2f duration으로 오는 속도
    bool isTracking;

    void OnEnable()
    {
        transform.localScale = Vector3.one;
        isTracking = false;
    }

    public void SetItemPos(Vector3 pos)
    {
        transform.position = pos;
    }

    // 아이템 떨어지는 효과
    public void DropItem()
    {
        Vector3 myPos = transform.position;

        float ranPosX = Random.Range(0.1f, 0.3f);
        ranPosX *= Random.value < 0.5f ? -1 : 1;

        Vector3 ranPos = new Vector3(ranPosX, Random.Range(-0.3f, -0.1f), 0);
        Vector3 endPos = myPos + ranPos;

        Vector3 midPoint = (myPos + endPos) / 2;
        midPoint.y += arcHeight;

        Vector3[] path = { myPos, midPoint, endPos };
        transform.DOPath(path, 0.3f, PathType.CatmullRom);
    }

    // 아이템 루팅 효과
    public void 
        PickUpItem()
    {
        isTracking = true;

        Vector3 playerPos = GameManager.instance.player.transform.position;

        float duration = Vector3.Distance(playerPos, transform.position) / pickupSpeed;
        duration = Mathf.Clamp(duration, 0.3f, 1f); // 너무 멀리있는 오브젝트는 시간 보정

        Sequence sequence = DOTween.Sequence();
        Tween moveTween = transform.DOMove(playerPos, duration).SetEase(Ease.InQuint);
        Tween scaleTween = transform.DOScale(0f, duration).SetEase(Ease.InQuint);

        sequence.Append(moveTween)
                .Join(scaleTween)
                .OnComplete(OnPickupComplete);
    }

    void OnPickupComplete()
    {
        GameManager.instance.PickUpItem(itemData);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTracking)
            return;

        PickUpItem();
    }

}
