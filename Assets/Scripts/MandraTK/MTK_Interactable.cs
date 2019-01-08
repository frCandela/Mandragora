using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_Interactable : MonoBehaviour
{
    [HideInInspector] public bool m_grabbed = false;

    [SerializeField] public bool isGrabbable = true;
    [SerializeField] public bool isDistanceGrabbable = true;
    [SerializeField] public bool isDroppable = true;
    [HideInInspector] public MTK_JointType jointType = null;

    [SerializeField] AK.Wwise.Event m_wOnUseStart;
    [SerializeField] UnityEvent m_onUseStart;
    [SerializeField] AK.Wwise.Event m_wOnUseStop;
    [SerializeField] UnityEvent m_onUseStop;
    [SerializeField] AK.Wwise.Event m_wOnGrabStart;
    [SerializeField] UnityEvent m_onGrabStart;
    [SerializeField] AK.Wwise.Event m_wOnGrabStop;
    [SerializeField] UnityEvent m_onGrabSop;

    public bool isDistanceGrabbed = false;

    public bool UseEffects
    {
        set
        {
            foreach (var effect in GetComponents<Effect>())
                effect.enabled = value;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        jointType = GetComponent<MTK_JointType>();
        if (isGrabbable && !jointType)
            jointType = gameObject.AddComponent<MTK_JointType_Fixed>();

        if(isGrabbable && isDistanceGrabbable)
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

            UseEffects = true;
        }

        m_grabbed = input;
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
