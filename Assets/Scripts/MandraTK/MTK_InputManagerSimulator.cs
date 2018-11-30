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
                onPrimaryInputPressed.Invoke();
            if (Input.GetMouseButtonUp(0))
                onPrimaryInputReleased.Invoke();
            if (Input.GetKey(KeyCode.Alpha1))
                transform.Rotate(Vector3.forward);
            if (Input.GetKey(KeyCode.Alpha2))
                transform.Rotate(-Vector3.forward);
        }
        if (m_hand == Hand.right)
        {
            if (Input.GetMouseButtonDown(1))
                onPrimaryInputPressed.Invoke();
            if (Input.GetMouseButtonUp(1))
                onPrimaryInputReleased.Invoke();
            if (Input.GetKey(KeyCode.Alpha3))
                transform.Rotate(Vector3.forward);
            if (Input.GetKey(KeyCode.Alpha4))
                transform.Rotate(-Vector3.forward);
        }
    }

}
