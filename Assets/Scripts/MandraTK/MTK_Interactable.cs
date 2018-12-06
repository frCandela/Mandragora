using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_Interactable : MonoBehaviour
{
    [SerializeField] public bool isGrabbable = true;
    [HideInInspector] public MTK_JointType jointType = null;

    [SerializeField] AK.Wwise.Event m_wOnUseStart;
    [SerializeField] UnityEvent m_onUseStart;
    [SerializeField] AK.Wwise.Event m_wOnUseStop;
    [SerializeField] UnityEvent m_onUseStop;
    [SerializeField] AK.Wwise.Event m_wOnGrabStart;
    [SerializeField] UnityEvent m_onGrabStart;
    [SerializeField] AK.Wwise.Event m_wOnGrabStop;
    [SerializeField] UnityEvent m_onGrabSop;

    // Use this for initialization
    void Awake ()
    {
        jointType = GetComponent<MTK_JointType>();
        if (isGrabbable && !jointType)
            jointType = gameObject.AddComponent<MTK_JointType_Fixed>();

        if(isGrabbable)
            MTK_InteractiblesManager.Instance.Subscribe(this);
    }

    public void Grab(bool input)
    {
        if (input)
        {
            m_onGrabStart.Invoke();
            m_wOnGrabStart.Post(gameObject);
        }
        else
        {
            m_onGrabSop.Invoke();
            m_wOnGrabStop.Post(gameObject);

            m_onUseStop.Invoke();
            m_wOnUseStop.Post(gameObject);
        }
    }

    public void Use(bool input)
    {
        if(input)
        {
            m_onUseStart.Invoke();
            m_wOnUseStart.Post(gameObject);
        }
        else
        {
            m_onUseStop.Invoke();
            m_wOnUseStop.Post(gameObject);
        }
    }
}
