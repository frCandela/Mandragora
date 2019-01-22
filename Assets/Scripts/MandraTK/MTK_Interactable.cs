using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_Interactable : MonoBehaviour
{
    [SerializeField] public bool isDistanceGrabbable = true;
    [SerializeField] public bool isDroppable = true;

    [SerializeField] private bool m_isGrabbable = true;
    public bool isGrabbable
    {
        get { return m_isGrabbable; }
        set
        {
            if (value != m_isGrabbable)
            {
                m_isGrabbable = value;
                onIsGrabbableChange.Invoke(this);
            }
        }
    }
    public UnityEventMTK_Interactable onIsGrabbableChange = new UnityEventMTK_Interactable();

    public MTK_JointType jointType { get { return m_joints[m_indexJointUsed]; } }
    private MTK_JointType[] m_joints;
    private int m_indexJointUsed = 0;
    public int IndexJointUsed { get { return m_indexJointUsed; } set { m_indexJointUsed = Mathf.Clamp(value, 0, m_joints.Length - 1); } }

    [SerializeField] AK.Wwise.Event m_wOnUseStart;
    [SerializeField] UnityEvent m_onUseStart;
    [SerializeField] AK.Wwise.Event m_wOnUseStop;
    [SerializeField] UnityEvent m_onUseStop;
    [SerializeField] AK.Wwise.Event m_wOnGrabStart;
    [SerializeField] UnityEvent m_onGrabStart;
    [SerializeField] AK.Wwise.Event m_wOnGrabStop;
    [SerializeField] UnityEvent m_onGrabSop;

    [HideInInspector] public bool isDistanceGrabbed = false;

    public bool UseEffects
    {
        set
        {
            foreach (var effect in GetComponents<Effect>())
            {
                if( effect.affectsRigidbody)
                {
                    effect.enabled = value;
                }
            }
        }
    }

    // Use this for initialization
    void Awake ()
    {
        m_onUseStart = new UnityEvent();
        m_onUseStop = new UnityEvent();
        m_onGrabStart = new UnityEvent();
        m_onGrabSop = new UnityEvent();
        m_wOnUseStart = new AK.Wwise.Event();
        m_wOnUseStop = new AK.Wwise.Event();
        m_wOnGrabStart = new AK.Wwise.Event();
        m_wOnGrabStop = new AK.Wwise.Event();

        m_joints = GetComponents<MTK_JointType>();
        if (m_joints.Length == 0)
            m_joints = new[]{ gameObject.AddComponent<MTK_JointType_Fixed>()};

        MTK_InteractiblesManager.Instance.Subscribe(this);        
    }

    private void OnDestroy()
    {
        if(MTK_InteractiblesManager.Instance)
            MTK_InteractiblesManager.Instance.UnSubscribe(this);
    }

    public virtual void Grab(bool input)
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
