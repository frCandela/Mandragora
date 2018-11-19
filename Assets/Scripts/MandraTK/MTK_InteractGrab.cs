using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(MTK_InputManager))]
public class MTK_InteractGrab : MonoBehaviour
{
    public MTK_Interactable objectGrabbed = null;

    private MTK_Interactable m_objectInTrigger = null;
    private MTK_Setup m_setup;
    private MTK_InputManager m_inputManager;

    [SerializeField] private Hand hand;
    public enum Hand {  left, right }

    private void Start()
    {
        m_setup = FindObjectOfType<MTK_Manager>().activeSetup;
        m_inputManager = GetComponent<MTK_InputManager>();

        m_inputManager.onPrimaryInputPressed.AddListener(InputPressed);
        m_inputManager.onPrimaryInputReleased.AddListener(InputReleased);
    }

    void InputPressed()
    {
        if (m_objectInTrigger)
        {
            Grab(m_objectInTrigger);
        }
    }

    void InputReleased()
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

            rb.velocity = m_inputManager.GetVelocity();
            rb.angularVelocity = m_inputManager.GetAngularVelocity();

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
