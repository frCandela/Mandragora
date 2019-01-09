using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class MTK_InteractHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Outliner m_outliner = null;
    [SerializeField] Animator m_handAnimator = null;

    [Header("Status")]
    [SerializeField] public MTK_Interactable m_grabbed = null;

    [Header("Events")]
    public UnityEventMTK_Interactable m_onTouchInteractable;
    public UnityEventMTK_Interactable m_onUnTouchInteractable;
    public UnityEventBool m_onUseFail;

    private List<MTK_Interactable> m_objectsInTrigger = new List<MTK_Interactable>(3);
    private MTK_Setup m_setup;
    private MTK_InputManager m_inputManager;


    public MTK_InputManager inputManager { get { return m_inputManager; } }
    private MTK_Interactable m_closest;
    public MTK_Interactable Closest
    {
        get {return m_closest;}
        set
        {
            if(value != m_closest)
                m_closest = value;
        }
    }


    private void Start()
    {
        m_setup = FindObjectOfType<MTK_Manager>().activeSetup;

        m_inputManager = GetComponentInParent<MTK_InputManager>();

        if(m_outliner)
        {
            // m_onTouchInteractable.AddListener(m_outliner.OultineOn);
            // m_onUnTouchInteractable.AddListener(m_outliner.OultineOff);
        }
    }

    private void FixedUpdate()
    {
        m_closest = GetClosestInteractable();
    }

    public void TryGrab(bool input)
    {
        if( input)
        {
            Grab(m_closest);            
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
        else if(m_closest)
            m_closest.Use(input);
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
            obj.Grab(true);

            if (obj.jointType.Used())
                obj.jointType.RemoveJoint();

            obj.jointType.onJointBreak.AddListener(Release);
            obj.jointType.JoinWith(gameObject);
            m_grabbed = obj;

            m_outliner.OultineOff(m_grabbed);

            m_handAnimator.SetBool("Visible", false);

            m_handAnimator.SetBool("Grab", true);
        }
    }

    void Release()
    {
        if(m_grabbed)
        {
            m_grabbed.Grab(false);

            m_grabbed.jointType.onJointBreak.RemoveListener(Release);
            m_grabbed.jointType.RemoveJoint();
            
            if(m_grabbed.jointType.rigidbody)
            {
                m_grabbed.jointType.rigidbody.velocity = m_inputManager.GetVelocity();
                m_grabbed.jointType.rigidbody.angularVelocity = m_inputManager.GetAngularVelocity();
            }

            m_grabbed.jointType.onJointBreak.RemoveListener(Release);
            m_grabbed = null;
            m_handAnimator.SetBool("Visible", true);
            m_handAnimator.SetBool("Grab", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody)
        {
            MTK_Interactable candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (candidate)
            {
                m_objectsInTrigger.Add(candidate);
                m_onTouchInteractable.Invoke(candidate);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody)
        {
            MTK_Interactable candidate = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (candidate)
            {
                m_objectsInTrigger.Remove(candidate);
                m_onUnTouchInteractable.Invoke(candidate);
            }
        }
    }

    MTK_Interactable GetClosestInteractable()
    {
        float minDistance = float.MaxValue,
                tmpDist;

        MTK_Interactable result = null;

        foreach (MTK_Interactable candidate in m_objectsInTrigger)
        {
            tmpDist = Vector3.Distance(transform.position, candidate.transform.position);

            if(tmpDist < minDistance)
                result = candidate;
        }

        return result;
    }
}
