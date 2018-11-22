using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(MTK_InputManager))]
public class MTK_InteractHand : MonoBehaviour
{
    public MTK_Interactable m_grabbed = null;

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
            {
                if(m_closest)
                    m_closest.GetComponent<MeshRenderer>().material.color = Color.white;

                m_closest = value;
                m_closest.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }
    }

    private void Start()
    {
        m_setup = FindObjectOfType<MTK_Manager>().activeSetup;
        m_inputManager = GetComponent<MTK_InputManager>();

        m_inputManager.onGrabInput.AddListener(onGrabPressed);
    }

    private void FixedUpdate()
    {
        m_closest = GetClosestInteractable();
    }

    void onGrabPressed(bool input)
    {
        if(input)
        {
            if (m_closest)
                Grab(m_closest.GetComponent<MTK_Interactable>());
        }
        else
        {
            if(m_grabbed)
                Release();
        }
    }

    private void OnJointBreak(float breakForce)
    {
        print("OnJointBreak");
        if (m_grabbed)
        {
            Release();
        }
    }

    void Grab( MTK_Interactable obj)
    {
        if( obj.jointType.Used())
        {
            obj.jointType.RemoveJoint();
            print("remove");
        }
        obj.jointType.onJointBreak.AddListener(Release);
        obj.jointType.JoinWith(gameObject);
        m_grabbed = obj;
    }

    void Release()
    {
        if(m_grabbed)
        {
            print("Release");
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
            m_objectsInTrigger.Add(candidate);
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_Interactable candidate = other.GetComponent<MTK_Interactable>();
        
        if(candidate)
        {
            candidate.GetComponent<MeshRenderer>().material.color = Color.white;
            m_objectsInTrigger.Remove(candidate);
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
