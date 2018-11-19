using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public abstract class MTK_Setup : MonoBehaviour
{
    [HideInInspector] public UnityEvent onPrimaryInputLeftPressed;
    [HideInInspector] public UnityEvent onPrimaryInputLeftReleased;
    [HideInInspector] public UnityEvent onPrimaryInputRightPressed;
    [HideInInspector] public UnityEvent onPrimaryInputRightReleased;

    public abstract Vector3 GetAngularVelocityRight();
    public abstract Vector3 GetAngularVelocityLeft( );
    public abstract Vector3 GetVelocityRight();
    public abstract Vector3 GetVelocityLeft();

    public abstract void UpdateSettings();
}
