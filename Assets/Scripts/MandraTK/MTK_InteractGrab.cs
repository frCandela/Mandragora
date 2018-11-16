using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MTK_InteractGrab : MonoBehaviour
{
    public enum InputButtons { Trigger, Grip, Pad };
    public InputButtons inputButton = InputButtons.Trigger;

    public MTK_Interactable objectGrabbed = null;

    private MTK_Interactable m_objectInTrigger = null;
    private SteamVR_TrackedController m_trackedController = null;

    // Use this for initialization
    void Awake ()
    {
        m_trackedController = GetComponent<SteamVR_TrackedController>();
        SetInputs();

    }

    void SetInputs()
    {
        switch(inputButton)
        {
            case InputButtons.Trigger:
                m_trackedController.TriggerClicked += InputPressed;
                m_trackedController.TriggerUnclicked += InputReleased;
                break;
            case InputButtons.Grip:
                m_trackedController.Gripped += InputPressed;
                m_trackedController.Ungripped += InputReleased;
                break;
            case InputButtons.Pad:
                m_trackedController.PadClicked += InputPressed;
                m_trackedController.PadUnclicked+= InputReleased;
                break;
        }
    }

    void InputPressed( object sender, ClickedEventArgs args)
    {
        if (m_objectInTrigger)
        {
            Grab(m_objectInTrigger);
        }
    }

    void InputReleased(object sender, ClickedEventArgs args)
    {
        if(objectGrabbed)       
            Release();        
    }

    void JointFailed(MTK_JointType joint)
    {
        objectGrabbed.jointType.onJointBreak.RemoveListener(JointFailed);
        Release();        
    }

    void Grab( MTK_Interactable obj)
    {
        if (obj.jointType.Used())
            obj.jointType.RemoveJoint();

        objectGrabbed = obj;
        if (!objectGrabbed.jointType.JoinWith(gameObject))
            print("zob");
        
        objectGrabbed.jointType.onJointBreak.AddListener(JointFailed);
    }

    void Release()
    {
        if(objectGrabbed)
        {
            Rigidbody rb = objectGrabbed.GetComponent<Rigidbody>();
            objectGrabbed.jointType.RemoveJoint();
            rb.velocity = m_trackedController.GetVelocity();
            rb.angularVelocity = m_trackedController.GetAngularVelocity();
            objectGrabbed = null;
        }
    }

    void EvaluateTrigger(GameObject obj)
    {
        MTK_Interactable interactable = obj.GetComponent<MTK_Interactable>();
        if (interactable && interactable.isGrabbable && interactable != objectGrabbed)
        {
            if (m_objectInTrigger)
            {
                if (Vector3.SqrMagnitude(interactable.transform.position - transform.position) < Vector3.SqrMagnitude(m_objectInTrigger.transform.position - transform.position))
                {
                    interactable.GetComponent<MeshRenderer>().material.color = Color.blue;
                    m_objectInTrigger.GetComponent<MeshRenderer>().material.color = Color.white;
                    m_objectInTrigger = interactable;
                }
            }
            else
            {
                m_objectInTrigger = interactable;
                m_objectInTrigger.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EvaluateTrigger(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        EvaluateTrigger(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_objectInTrigger && other.gameObject == m_objectInTrigger.gameObject)
        {
            m_objectInTrigger.GetComponent<MeshRenderer>().material.color = Color.white;
            m_objectInTrigger = null;
        }
    }
}
