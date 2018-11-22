using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSteamVR : MTK_Setup
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    // Use this for initialization
    void Awake()
    {
        Util.EditorAssert(head != null, "Please select a head gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(leftHand != null, "Please select a leftHand gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(rightHand != null, "Please select a rightHand gameobject in the MTK_SetupSteamVR");
    }

    public override void UpdateSettings()
    {
        #if UNITY_EDITOR
        PlayerSettings.virtualRealitySupported = true;
        #endif
    }
}
