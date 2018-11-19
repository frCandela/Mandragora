using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSimulator : MTK_Setup
{
    public override Vector3 GetAngularVelocityRight() { return Vector3.zero; }
    public override Vector3 GetAngularVelocityLeft() { return Vector3.zero; }
    public override Vector3 GetVelocityRight() { return Vector3.zero; }
    public override Vector3 GetVelocityLeft() { return Vector3.zero; }

    /* private void Update()
     {
         if (Input.GetMouseButtonDown(0))
             print("test");
     }*/

    public override void UpdateSettings()
    {

        PlayerSettings.virtualRealitySupported = false;
    }
}
