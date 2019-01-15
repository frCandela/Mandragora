using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InputButtons
{
    Trigger,
    Grip,
    Pad
};

public abstract class MTK_InputManager : MonoBehaviour
{
    public UnityEventBool m_onTrigger, m_onGrip, m_onPad;

    protected MTK_InteractHand m_interractHand;

    public abstract Vector3 GetAngularVelocity();
    public abstract Vector3 GetVelocity();
    public abstract void Haptic(float Time);

    protected void OnEnable()
    {
        m_interractHand = GetComponentInChildren<MTK_InteractHand>();
    }
}
