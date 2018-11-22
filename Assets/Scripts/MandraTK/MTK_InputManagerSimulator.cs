using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_InputManagerSimulator : MTK_InputManager
{
    public enum Hand {left, right}
    [SerializeField] private Hand m_hand;


    public override Vector3 GetAngularVelocity() { return Vector3.zero; }
    public override Vector3 GetVelocity() { return Vector3.zero; }

    private void Update()
    {
        if (m_hand == Hand.left)
        {
            if (Input.GetMouseButtonDown(0))
                onGrabInput.Invoke(true);
            if (Input.GetMouseButtonUp(0))
                onGrabInput.Invoke(false);
        }
        if (m_hand == Hand.right)
        {
            if (Input.GetMouseButtonDown(1))
                onGrabInput.Invoke(true);
            if (Input.GetMouseButtonUp(1))
                onGrabInput.Invoke(false);
        }
    }

    public override void Haptic(float Time)
    {
        print("bzz bzz");
    }
}
