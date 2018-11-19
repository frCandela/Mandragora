using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTK_SetupSteamVR : MTK_Setup
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    private SteamVR_TrackedController m_trackedControllerLeftHand = null;
    private SteamVR_TrackedController m_trackedControllerRightHand = null;

    public enum InputButtons { Trigger, Grip, Pad };
    public InputButtons inputButton = InputButtons.Trigger;

    public override Vector3 GetAngularVelocityRight() { return m_trackedControllerRightHand.GetAngularVelocity(); }
    public override Vector3 GetAngularVelocityLeft() {  return m_trackedControllerLeftHand.GetAngularVelocity(); }
    public override Vector3 GetVelocityRight() { return m_trackedControllerRightHand.GetVelocity();  }
    public override Vector3 GetVelocityLeft() { return m_trackedControllerLeftHand.GetVelocity(); }

    // Use this for initialization
    void Awake()
    {
        Util.EditorAssert(head != null, "Please select a head gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(leftHand != null, "Please select a leftHand gameobject in the MTK_SetupSteamVR");
        Util.EditorAssert(rightHand != null, "Please select a rightHand gameobject in the MTK_SetupSteamVR");

        Camera.main.transform.parent = head.transform;
        m_trackedControllerLeftHand = leftHand.GetComponent<SteamVR_TrackedController>();
        m_trackedControllerRightHand = rightHand.GetComponent<SteamVR_TrackedController>();
    }

    public override void UpdateSettings()
    {
        PlayerSettings.virtualRealitySupported = true;
    }
    /*void SetInputs(SteamVR_TrackedController trackedController)
    {
        switch (inputButton)
        {
            case InputButtons.Trigger:
                trackedController.TriggerClicked += InputPressed;
                trackedController.TriggerUnclicked += InputReleased;
                break;
            case InputButtons.Grip:
                trackedController.Gripped += InputPressed;
                trackedController.Ungripped += InputReleased;
                break;
            case InputButtons.Pad:
                trackedController.PadClicked += InputPressed;
                trackedController.PadUnclicked += InputReleased;
                break;
        }
    }*/
}
