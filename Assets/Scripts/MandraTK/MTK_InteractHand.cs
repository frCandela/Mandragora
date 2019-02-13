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
    public MTK_Interactable ObjectInTrigger
    {
        get { return m_objectInTrigger; }
        set
        {
            // Unselect if old
            if (m_objectInTrigger)
            {
                m_objectInTrigger.Outline = false;
                m_onUnTouchInteractable.Invoke(m_objectInTrigger);
                m_objectInTrigger = null;            
            }
            
            m_objectInTrigger = value;

            if(m_objectInTrigger)
            {
                m_objectInTrigger.Outline = true;
                m_onTouchInteractable.Invoke(m_objectInTrigger);
            }
        }
    }

    private MTK_InputManager m_inputManager;
    private MTK_Setup m_setup;
    private MTK_JointType grabbedJoint;
    private TelekinesisPointer m_telekinesis;

    [HideInInspector] public MTK_Interactable m_grabbed = null;

    public MTK_InputManager inputManager { get { return m_inputManager; } }

    private void Awake()
    {
        foreach(MTK_Interactable interactable in FindObjectsOfType<MTK_Interactable>())
        {
            interactable.onIsGrabbableChange.AddListener(onIsGrabbableChange);
        }
    }

    private void Update()
    {
        m_handAnimator.SetFloat("BlendGrab", m_inputManager.GetTriggerValue());
    }

    private void Start()
    {
        m_inputManager = GetComponentInParent<MTK_InputManager>();
        m_setup = GetComponentInParent<MTK_Setup>();
        m_telekinesis = GetComponent<TelekinesisPointer>();


        m_inputManager.m_onPad.AddListener(OnPad);
    }

    public void OnPad(bool input)
    {
        m_handAnimator.SetBool("ThumbPressed", input);        
    }

    public void TryGrab(bool input)
    {
        if(input)
            Grab(ObjectInTrigger);
        else
            Release();
    }

    public void TryUse(bool input)
    {
        if(m_grabbed)
            m_grabbed.Use(input);
        else if(ObjectInTrigger)
            ObjectInTrigger.Use(input);
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
            // if(!obj.GetComponent<ScaleEffect>() && !obj.GetComponent<IcoSegment>())
            //     obj.transform.position = transform.position;
            
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
            m_telekinesis.Active = false;

            m_handAnimator.SetBool("Visible", false);
            m_handAnimator.SetBool("Attract", true);

            m_setup.NotifyGrab(m_grabbed);

            // ObjectInTrigger = null;
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
            m_telekinesis.Active = true;
           
            m_handAnimator.SetBool("Visible", true);
            m_handAnimator.SetBool("Attract", false);

            m_onReleaseInteractable.Invoke(m_grabbed);

            m_setup.NotifyRelease(m_grabbed);
            m_grabbed = null;

            // ObjectInTrigger = null;
        }
    }

    void onIsGrabbableChange(MTK_Interactable interactable)
    {
        if (interactable.isGrabbable)
        {
            //m_objectsInTrigger.RemoveAll(x => x == interactable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MTK_Interactable candidate = GetCandidate(other);

        if (candidate && candidate.isGrabbable)
            ObjectInTrigger = candidate;
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_Interactable candidate = GetCandidate(other);

        if (candidate && candidate == ObjectInTrigger)
            ObjectInTrigger = null;
    }

    MTK_Interactable GetCandidate(Collider other)
    {
        IcoSegment seg = other.gameObject.GetComponent<IcoSegment>();
        if (seg && fingerCollider && seg.IsInside(fingerCollider.position, other))
            return null;

        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        if(!(candidate && candidate.isGrabbable) && other.attachedRigidbody)
            candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();

        return candidate;
    }
}