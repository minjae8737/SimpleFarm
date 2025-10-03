using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHandler
{
    public int Id { get; private set; }
    public float TimeLimit { get; set; }
    public Action OnTimerEnd;

    public TimerHandler(int id, float timeLimit)
    {
        Id = id;
        TimeLimit = timeLimit;
    }
}
