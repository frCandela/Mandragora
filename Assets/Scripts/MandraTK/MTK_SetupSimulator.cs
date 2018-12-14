using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSimulator : MTK_Setup
{

    void Awake()
    {
        CheckSetup();

        Camera.main.transform.localRotation = Quaternion.identity;
    }

    public override void UpdateSettings()
    {
        #if UNITY_EDITOR
        PlayerSettings.virtualRealitySupported = false;
        #endif
    }
}
