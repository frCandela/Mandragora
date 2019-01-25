using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class MTK_InteractHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator m_handAnimator = null;

    [Header("Events")]
    public UnityEventMTK_Interactable m_onTouchInteractable;
    public UnityEventMTK_Interactable m_onUnTouchInteractable;
    public UnityEventMTK_Interactable m_onReleaseInteractable;
    public UnityEventBool m_onUseFail;

    public MTK_Interactable m_objectInTrigger;
    private MTK_InputManager m_inputManager;
    private MTK_JointType grabbedJoint;

    [HideInInspector] public MTK_Interactable m_grabbed = null;

    public MTK_InputManager inputManager { get { return m_inputManager; } }

    private void Awake()
    {
        foreach(MTK_Interactable interactable in FindObjectsOfType<MTK_Interactable>())
        {
            interactable.onIsGrabbableChange.AddListener(onIsGrabbableChange);
        }
    }

    void onIsGrabbableChange( MTK_Interactable interactable)
    {
        if( interactable.isGrabbable)
        {
            //m_objectsInTrigger.RemoveAll(x => x == interactable);
        }
    }

    private void Start()
    {
        m_inputManager = GetComponentInParent<MTK_InputManager>();
    }

    public void TryGrab(bool input)
    {
        if( input)
        {
            Grab(m_objectInTrigger);            
        }
        else
        {
            Release();
        }
    }

    public void TryUse(bool input)
    {
        if(m_grabbed)
            m_grabbed.Use(input);
        else if(m_objectInTrigger)
            m_objectInTrigger.Use(input);
        else
            m_onUseFail.Invoke(input);
    }

    void OnJointBreak(float breakForce)
    {
        if (m_grabbed)
            Release();
    }

    public void Grab(MTK_Interactable obj)
    {
        if(obj)
        {
            if (obj.jointType.Used())
                obj.jointType.RemoveJoint();

            obj.jointType.onJointBreak.AddListener(Release);

            if( !obj.jointType.JoinWith(gameObject))
            {
                obj.jointType.onJointBreak.RemoveListener(Release);
                return;
            }

            obj.Grab(true);

            m_grabbed = obj;
            grabbedJoint = obj.jointType;

            m_grabbed.Outline = true;

            m_handAnimator.SetBool("Visible", false);

            m_handAnimator.SetBool("Grab", true);
        }
    }

    void Release()
    {
        if(m_grabbed)
        {
            m_grabbed.Grab(false);

            grabbedJoint.onJointBreak.RemoveListener(Release);
            grabbedJoint.RemoveJoint();
            
            if(grabbedJoint.rigidbody)
            {
                grabbedJoint.rigidbody.velocity = m_inputManager.GetVelocity();
                grabbedJoint.rigidbody.angularVelocity = m_inputManager.GetAngularVelocity();
            }

            grabbedJoint.onJointBreak.RemoveListener(Release);
           
            m_handAnimator.SetBool("Visible", true);
            m_handAnimator.SetBool("Grab", false);

            m_onReleaseInteractable.Invoke(m_grabbed);
            m_grabbed = null;
            m_objectInTrigger = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        if (  (!candidate || !candidate.isGrabbable) && other.attachedRigidbody)
            candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();

        if (candidate && candidate.isGrabbable)
        {
            m_objectInTrigger = candidate;
            m_onTouchInteractable.Invoke(candidate);

            candidate.Outline = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        if ((!candidate || !candidate.isGrabbable) && other.attachedRigidbody)
            candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();

        if (candidate )
        {
            candidate.Outline = false;
            m_onUnTouchInteractable.Invoke(candidate);
            if(candidate == m_objectInTrigger)
            {
                m_objectInTrigger = null;
                
            }
        }
    }    
}
