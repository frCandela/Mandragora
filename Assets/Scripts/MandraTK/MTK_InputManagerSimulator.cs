using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_InputManagerSimulator : MTK_InputManager
{
    public override Vector3 GetAngularVelocity() { return Vector3.zero; }
    public override Vector3 GetVelocity() { return Vector3.zero; }

}
