using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_InputManagerSimulator : MTK_InputManager
{
    // public enum Hand {left, right}
    // [SerializeField] private Hand m_hand;

    public override Vector3 GetAngularVelocity() { return Vector3.zero; }
    public override Vector3 GetVelocity() { return Vector3.zero; }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            onInput.Invoke(InputButtons.Grip, true);
        if (Input.GetMouseButtonUp(0))
            onInput.Invoke(InputButtons.Grip,false);
            
        if (Input.GetMouseButtonDown(1))
            onInput.Invoke(InputButtons.Trigger, true);
        if (Input.GetMouseButtonUp(1))
            onInput.Invoke(InputButtons.Trigger, false);

        if (Input.GetMouseButtonDown(1))
            onInput.Invoke(InputButtons.Pad, true);
        if (Input.GetMouseButtonUp(1))
            onInput.Invoke(InputButtons.Pad, false);
    }

    public override void Haptic(float Time)
    {
        print("bzz bzz");
    }
}
