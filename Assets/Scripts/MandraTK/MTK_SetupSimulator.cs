using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSimulator : MTK_Setup
{
    public override void UpdateSettings()
    {
        PlayerSettings.virtualRealitySupported = false;
    }
}
