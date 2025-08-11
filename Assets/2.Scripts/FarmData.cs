using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FarmData
{
    public string name;
    [Header("최대 농장 레벨")]
    public int maxFarmLevel;
    [Header("농장 기본 골드")]
    public int basicFarmGold;
    [Header("레벨별 농장 골드 증가율")]
    public float farmGoldPer;

    [Space(10)]
    [Header("최대 자동 생산 확률 레벨")]
    public int maxAutoProduceCanceLevel;
    [Header("기본 자동 생산 확률(%)")]
    public float basicAutoProduceCance;
    [Header("레벨당 생산 확률 증가(%)")]
    public float autoProduceChancePer;
    [Header("자동 생산 확률 기본 골드")]
    public int basicAutoProduceChanceGold;
    [Header("레벨별 자동 생산 확률 골드 증가율")]
    public float autoProduceChanceGoldPer;


    [Space(10)]
    [Header("최대 생산 시간 레벨")]
    public int maxProduceCooldownLevel;
    [Header("기본 생산 시간")]
    public float basicProduceCooldown;
    [Header("레벨당 생산 시간 감소(sec)")]
    public float produceCooldownPer;
    [Header("생산 시간 감소 기본 골드")]
    public int basicProduceCooldownGold;
    [Header("레벨별 생산 시간 감소 골드 증가율")]
    public float produceCooldownGoldPer;

}
