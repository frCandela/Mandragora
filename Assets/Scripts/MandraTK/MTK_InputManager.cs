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
    public UnityEventInputBool onInput;

    public abstract Vector3 GetAngularVelocity();
    public abstract Vector3 GetVelocity();
    public abstract void Haptic(float Time);
}
