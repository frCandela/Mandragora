﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSimulator : MTK_Setup
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    void Awake()
    {
        Util.EditorAssert(head != null, "Please select a head gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(leftHand != null, "Please select a leftHand gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(rightHand != null, "Please select a rightHand gameobject in the MTK_SetupSteamVR");

        Camera.main.transform.localRotation = Quaternion.identity;
    }

    public override void UpdateSettings()
    {
        PlayerSettings.virtualRealitySupported = false;
    }
}
