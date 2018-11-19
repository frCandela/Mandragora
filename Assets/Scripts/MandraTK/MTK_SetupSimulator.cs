using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_SetupSimulator : MTK_Setup
{
    public override Vector3 AngularVelocityRight() { return Vector3.zero; }
    public override Vector3 AngularVelocityLeft() { return Vector3.zero; }
    public override Vector3 VelocityRight() { return Vector3.zero; }
    public override Vector3 VelocityLeft() { return Vector3.zero; }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            print("test");
    }
}
