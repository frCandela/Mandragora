using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedController))]
public class MTK_InputManagerSteamVR : MTK_InputManager
{
    public enum InputButtons { Trigger, Grip, Pad };
    [SerializeField] private InputButtons primaryInput = InputButtons.Trigger;

    private SteamVR_TrackedController m_trackedController = null;

    public override Vector3 GetAngularVelocity() { return m_trackedController.GetAngularVelocity(); }
    public override Vector3 GetVelocity() { return m_trackedController.GetVelocity(); }

    private void Awake()
    {
        m_trackedController = GetComponent<SteamVR_TrackedController>();
        SetInput();
    }

    void PrimaryInputPressed(object sender, ClickedEventArgs e)
    {
        onPrimaryInputPressed.Invoke();
    }
    void PrimaryInputReleased(object sender, ClickedEventArgs e)
    {
        onPrimaryInputReleased.Invoke();
    }

    void SetInput()
    {
        switch (primaryInput)
        {
            case InputButtons.Trigger:
                m_trackedController.TriggerClicked += PrimaryInputPressed;
                m_trackedController.TriggerUnclicked += PrimaryInputReleased;
                break;
            case InputButtons.Grip:
                m_trackedController.Gripped += PrimaryInputPressed;
                m_trackedController.Ungripped += PrimaryInputReleased;
                break;
            case InputButtons.Pad:
                m_trackedController.PadClicked += PrimaryInputPressed;
                m_trackedController.PadUnclicked += PrimaryInputReleased;
                break;
        }
    }
}
