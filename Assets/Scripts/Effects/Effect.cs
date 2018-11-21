using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    protected virtual void Awake()
    {
        ApplyEffect();
    }

    private void OnDestroy()
    {
        RemoveEffect();
    }

    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}

