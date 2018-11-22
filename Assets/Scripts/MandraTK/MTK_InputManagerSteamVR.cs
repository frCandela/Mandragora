using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedController))]
public class MTK_InputManagerSteamVR : MTK_InputManager
{
    private SteamVR_TrackedController m_trackedController = null;

    public override Vector3 GetAngularVelocity() { return m_trackedController.GetAngularVelocity(); }
    public override Vector3 GetVelocity() { return m_trackedController.GetVelocity(); }

    private void Awake()
    {
        m_trackedController = GetComponent<SteamVR_TrackedController>();
        SetInput();
    }

    void TriggerPressed(object sender, ClickedEventArgs e)
    {
        m_onTrigger.Invoke(true);
    }
    void TriggerReleased(object sender, ClickedEventArgs e)
    {
        m_onTrigger.Invoke(false);
    }

    void GripPressed(object sender, ClickedEventArgs e)
    {
        m_onGrip.Invoke(true);
    }
    void GripReleased(object sender, ClickedEventArgs e)
    {
        m_onGrip.Invoke(false);
    }

    void PadPressed(object sender, ClickedEventArgs e)
    {
        m_onPad.Invoke(true);
    }
    void PadReleased(object sender, ClickedEventArgs e)
    {
        m_onPad.Invoke(false);
    }

    void SetInput()
    {
        m_trackedController.TriggerClicked += TriggerPressed;
        m_trackedController.TriggerUnclicked += TriggerReleased;
        
        m_trackedController.Gripped += GripPressed;
        m_trackedController.Ungripped += GripReleased;

        m_trackedController.PadClicked += PadPressed;
        m_trackedController.PadUnclicked += PadReleased;
    }

    public override void Haptic(float Time)
    {
        SteamVR_Controller.Input((int)m_trackedController.controllerIndex).TriggerHapticPulse((ushort)(Time * 1000));
    }

    public override void Haptic(float Time)
    {
        SteamVR_Controller.Input((int)m_trackedController.controllerIndex).TriggerHapticPulse((ushort)(Time * 1000));
    }
}
