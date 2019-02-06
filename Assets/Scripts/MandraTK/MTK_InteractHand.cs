using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class MTK_InteractHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator m_handAnimator = null;
    public Transform fingerCollider = null;

    [Header("Events")]
    public UnityEventMTK_Interactable m_onTouchInteractable;
    public UnityEventMTK_Interactable m_onUnTouchInteractable;
    public UnityEventMTK_Interactable m_onReleaseInteractable;
    public UnityEventBool m_onUseFail;

    private MTK_Interactable m_objectInTrigger;
    public MTK_Interactable objectInTrigger { get { return m_objectInTrigger; }}
    private int m_nbContacts = 0;

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
            if(!obj.GetComponent<ScaleEffect>() && !obj.GetComponent<IcoSegment>())
                obj.transform.position = transform.position;
            
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

            inputManager.Haptic(1);

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

            RemoveObjectInTrigger();

            m_grabbed = null;
            m_objectInTrigger = null;
        }
    }

    void onIsGrabbableChange(MTK_Interactable interactable)
    {
        if (interactable.isGrabbable)
        {
            //m_objectsInTrigger.RemoveAll(x => x == interactable);
        }
    }

    void SetObjectInTrigger(MTK_Interactable interactable)
    {
        if( ! m_objectInTrigger)
        {
            interactable.Outline = true;
            m_onTouchInteractable.Invoke(interactable);
            m_objectInTrigger = interactable;
        }
        else
        {
            print("error SetObjectInTrigger");
        }
    }

    void RemoveObjectInTrigger()
    {
        if (m_objectInTrigger)
        {
            m_objectInTrigger.Outline = false;
            m_onUnTouchInteractable.Invoke(m_objectInTrigger);
            m_objectInTrigger = null;            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IcoSegment seg = other.gameObject.GetComponent<IcoSegment>();
        
        if (seg && fingerCollider && seg.IsInside(fingerCollider.position, other))
        {
            return;
        }

        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        if (candidate && candidate.isGrabbable)
        {
            RemoveObjectInTrigger();
            SetObjectInTrigger(candidate);
        }
        else if (other.attachedRigidbody)
        {
            candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (candidate && candidate.isGrabbable)
            {
                if(m_objectInTrigger == candidate)
                {
                    ++m_nbContacts;
                }
                else
                {
                    if (m_objectInTrigger)
                    {
                        RemoveObjectInTrigger();
                    }

                    SetObjectInTrigger(candidate);
                    m_nbContacts = 1;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IcoSegment seg = other.gameObject.GetComponent<IcoSegment>();
        if (seg && fingerCollider && seg.IsInside(fingerCollider.position, other))
        {
            return;
        }

        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        if (candidate && candidate.isGrabbable)
        {
            if( candidate == m_objectInTrigger)
            {
                RemoveObjectInTrigger();
            }
        }
        else if (other.attachedRigidbody)
        {
            candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (candidate && candidate.isGrabbable)
            {
                if (m_objectInTrigger == candidate)
                {
                    --m_nbContacts;
                    if (m_nbContacts <= 0)
                    {
                        RemoveObjectInTrigger();
                    }
                }
            }
        }
    }
}
