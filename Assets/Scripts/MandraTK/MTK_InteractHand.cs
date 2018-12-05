using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MTK_InteractHand : MonoBehaviour
{
    public MTK_Interactable m_grabbed = null;
    [SerializeField] Outliner m_outliner = null;

    public UnityEventMTK_Interactable m_onTouchInteractable;
    public UnityEventMTK_Interactable m_onUnTouchInteractable;

    private List<MTK_Interactable> m_objectsInTrigger = new List<MTK_Interactable>(3);
    private MTK_Setup m_setup;
    private MTK_InputManager m_inputManager;

    private MTK_Interactable m_closest;
    MTK_Interactable Closest
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
        m_inputManager.m_onTrigger.AddListener(OnTrigger);
        m_inputManager.m_onGrip.AddListener(OnGrip);
        m_inputManager.m_onPad.AddListener(OnPad);

        if(m_outliner)
        {
            m_onTouchInteractable.AddListener(m_outliner.OultineOn);
            m_onUnTouchInteractable.AddListener(m_outliner.OultineOff);
        }
    }

    private void FixedUpdate()
    {
        m_closest = GetClosestInteractable();
    }

    void OnTrigger(bool input)
    {
        if(input)
        {
            if (m_closest)
            {
                m_closest.Grab(input);

                if(m_closest.isGrabbable)
                    Grab(m_closest);
            }
        }
        else
        {
            if(m_grabbed)
            {
                m_grabbed.Grab(input);
                Release();
            }
        }
    }

    void OnGrip(bool input)
    {
        
    }

    void OnPad(bool input)
    {
        if(m_grabbed)
            m_grabbed.Use(input);
        else if(m_closest)
            m_closest.Use(input);
    }

    private void OnJointBreak(float breakForce)
    {
        if (m_grabbed)
            Release();
    }

    public void Grab( MTK_Interactable obj)
    {
        if( obj.jointType.Used())
            obj.jointType.RemoveJoint();

        obj.jointType.onJointBreak.AddListener(Release);
        obj.jointType.JoinWith(gameObject);
        m_grabbed = obj;
    }

    void Release()
    {
        if(m_grabbed)
        {
            m_grabbed.jointType.onJointBreak.RemoveListener(Release);
            m_grabbed.jointType.RemoveJoint();
            Rigidbody rb = m_grabbed.GetComponent<Rigidbody>();
            m_grabbed.jointType.onJointBreak.RemoveListener(Release);
            rb.velocity = m_inputManager.GetVelocity();
            rb.angularVelocity = m_inputManager.GetAngularVelocity();

            m_grabbed = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();

        if(candidate)
        {
            m_objectsInTrigger.Add(candidate);
            m_onTouchInteractable.Invoke(candidate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        
        if(candidate)
        {
            m_objectsInTrigger.Remove(candidate);
            m_onUnTouchInteractable.Invoke(candidate);
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
