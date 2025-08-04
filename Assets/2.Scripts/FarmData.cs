using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmData
{
    public string name;
    [Header("농장 레벨")]
    public int maxFarmLevel;
    [Header("자동 생산 확률 레벨")]
    public int maxAutoProduceCanceLevel;
    [Header("기본 자동 생산 확률")]
    public float basicAutoProduceCance;
    [Header("생산 시간 레벨")]
    public int maxProduceCooldownLevel;
    [Header("기본 생산 시간")]
    public float basicProduceCooldown;
}
