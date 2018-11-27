using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSteamVR : MTK_Setup
{
    // Use this for initialization
    void Awake()
    {
        CheckSetup();
    }

    public override void UpdateSettings()
    {
        #if UNITY_EDITOR
        PlayerSettings.virtualRealitySupported = true;
        #endif
    }
}
