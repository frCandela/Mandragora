using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MTK_InputManager : MonoBehaviour
{
    public UnityEvent onPrimaryInputPressed;
    public UnityEvent onPrimaryInputReleased;

    public abstract Vector3 GetAngularVelocity();
    public abstract Vector3 GetVelocity();
    public abstract void Haptic(float Time);
}
