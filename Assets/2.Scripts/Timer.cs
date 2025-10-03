using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private int timerId;
    private Dictionary<int, TimerHandler> timer;

    private void Awake()
    {
        timerId = 0;
        timer = new Dictionary<int, TimerHandler>();
    }

    public TimerHandler StartTimer(float timeLimit, Action onTimerEnd = null)
    {
        TimerHandler timerHandler = new TimerHandler(timerId++, timeLimit);
        timerHandler.OnTimerEnd = onTimerEnd;

        timer.Add(timerHandler.Id, timerHandler);
        StartCoroutine(StartTimer(timerHandler));
        
        return timerHandler;
    }

    private IEnumerator StartTimer(TimerHandler timerHandler)
    {
        while (timerHandler.TimeLimit > 0)
        {
            timerHandler.TimeLimit -= Time.deltaTime;
            yield return null;
        }

        timerHandler.OnTimerEnd?.Invoke();
        timer.Remove(timerHandler.Id);
    }
}