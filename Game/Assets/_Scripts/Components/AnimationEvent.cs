using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public UnityEvent onAttackFinish, onAttackPerform;

    public void AttackFinish()
    {
        onAttackFinish?.Invoke();
    }

    public void AttackPerform()
    {
        onAttackPerform?.Invoke();
    }
}
