using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_SetupSteamVR : MTK_Setup
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    private SteamVR_TrackedController m_trackedControllerLeftHand = null;
    private SteamVR_TrackedController m_trackedControllerRightHand = null;

    public enum InputButtons { Trigger, Grip, Pad };
    public InputButtons inputButton = InputButtons.Trigger;

    // Use this for initialization
    void Awake()
    {
        if (!leftHand)
            leftHand = new GameObject("leftHand");
        if (!rightHand)
            rightHand = new GameObject("rightHand");
        if (!head)
            head = new GameObject("head");

        leftHand.transform.parent = transform;
        rightHand.transform.parent = transform;
        head.transform.parent = transform;
        Camera.main.transform.parent = head.transform;

        m_trackedControllerLeftHand = leftHand.GetComponent<SteamVR_TrackedController>();
        m_trackedControllerRightHand = rightHand.GetComponent<SteamVR_TrackedController>();

        SetInputs(m_trackedControllerLeftHand);
        SetInputs(m_trackedControllerRightHand);
    }

    void SetInputs(SteamVR_TrackedController trackedController)
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
    }
}
