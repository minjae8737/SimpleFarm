using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    public event Action OnStateEnd;

    public void EndAnimation()
    {
        OnStateEnd?.Invoke();
    }
}
