using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class MTK_InteractHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Outliner m_outliner = null;

    [Header("Status")]
    [SerializeField] public MTK_Interactable m_grabbed = null;
    [SerializeField] public bool m_grabPressed = false;

    [Header("Events")]
    public UnityEventMTK_Interactable m_onTouchInteractable;
    public UnityEventMTK_Interactable m_onUnTouchInteractable;
    public UnityEventBool m_onUseFail;

    private List<MTK_Interactable> m_objectsInTrigger = new List<MTK_Interactable>(3);
    private MTK_Setup m_setup;
    private MTK_InputManager m_inputManager;


    public MTK_InputManager inputManager { get { return m_inputManager; } }
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

        m_grabPressed = input;
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
        }
    }

    void Release()
    {
        if(m_grabbed)
        {
            m_grabbed.Grab(false);

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
