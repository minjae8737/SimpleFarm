using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public Text goldText;
    public GameObject questPanel;
    public GameObject playerHp;
    Image palyerHpImg;
    Text playerHpText;

    public long gold;

    float recoveHpDuration = 3f; // 0 -> 1 까지 걸리는 시간

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ConvertGoldToText(gold);
            SetPlayerHp();
            SetPlayerHpText();
        }
        if (Input.GetKey(KeyCode.A))
        {
            RecorvePlayerHpEffect();
        }
    }

    public void Init()
    {
        palyerHpImg = playerHp.transform.GetChild(0).GetComponent<Image>();
        playerHpText = playerHp.transform.GetChild(1).GetComponent<Text>();
    }

    public void SetGoldText(long gold)
    {
        goldText.text = ConvertGoldToText(gold);
    }

    string ConvertGoldToText(long gold)
    {
        string goldStr = "";
        string[] postFixUnit = { "", "K", "M", "B" };

        int count = 0;
        double goldD = gold;

        while (goldD >= 1000)
        {
            goldD /= 1000;
            count++;
        }

        if (count >= postFixUnit.Length)
            count = postFixUnit.Length - 1;

        goldD = Math.Floor(goldD * 100) / 100;  // 반올림 방지
        goldStr = string.Format("{0:0.##}", goldD) + postFixUnit[count];

        return goldStr;
    }


    public void SetPlayerHp()
    {
        float playerHpAmount = (float)GameManager.instance.player.hp / GameManager.instance.player.maxHp;
        palyerHpImg.fillAmount = playerHpAmount;
    }

    public void SetPlayerHpText()
    {
        playerHpText.text = GameManager.instance.player.hp + " / " + GameManager.instance.player.maxHp;
    }

    public void RecorvePlayerHpEffect()
    {
        Sequence sequence = DOTween.Sequence();

        float effectSpeed = 1f / recoveHpDuration;
        float fillTime = (1f - palyerHpImg.fillAmount) / effectSpeed;

        Tween fillHpTween = palyerHpImg.DOFillAmount(1f, fillTime).SetEase(Ease.Linear);
        Tween scaleBiggerTween = playerHp.GetComponent<RectTransform>().DOScale(1.2f, 0.5f);
        Tween scaleSmallerTween = playerHp.GetComponent<RectTransform>().DOScale(1f, 0.5f);

        sequence.Append(fillHpTween)
        .Join(scaleBiggerTween)
        .Append(scaleSmallerTween);
    }


}
