using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public abstract class MTK_Setup : MonoBehaviour
{
    public UnityEvent primaryInputLeftPressed;
    public UnityEvent primaryInputLeftReseased;

    public UnityEvent primaryInputRightPressed;
    public UnityEvent primaryInputRightReleased;


    public abstract Vector3 AngularVelocityRight();
    public abstract Vector3 AngularVelocityLeft( );
    public abstract Vector3 VelocityRight();
    public abstract Vector3 VelocityLeft();
}
