using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_Interactable : MonoBehaviour
{
    [SerializeField] public bool isGrabbable = true;
    [HideInInspector] public MTK_JointType jointType = null;

    [SerializeField] UnityEvent m_onUseStart, m_onUseSop;
    [SerializeField] UnityEvent m_onGrabStart, m_onGrabSop;

    // Use this for initialization
    void Awake ()
    {
        if (isGrabbable && !jointType)
            jointType = gameObject.AddComponent<MTK_JointType_Fixed>();
    }

    public void Grab(bool input)
    {
        if(input)
            m_onGrabStart.Invoke();
        else
            m_onGrabSop.Invoke();
    }

    public void Use(bool input)
    {
        if(input)
            m_onUseStart.Invoke();
        else
            m_onUseSop.Invoke();
    }
}
