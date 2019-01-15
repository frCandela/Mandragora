using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MTK_JointType : MonoBehaviour
{
    public UnityEvent onJointBreak = new UnityEvent();
    public UnityEventBool onJointCreated = new UnityEventBool();

    [SerializeField, HideInInspector] protected Joint m_joint;
    [SerializeField,] protected GameObject m_connectedGameobject;
    [SerializeField, HideInInspector] public new Rigidbody rigidbody;

    public Joint joint { get { return m_joint; } }
    public GameObject connectedGameobject { get { return m_connectedGameobject; } }

    public bool JoinWith(GameObject other)
    {
        if(JointWithOverride(other))
        {
            onJointCreated.Invoke(true);
            m_connectedGameobject = other;
            return true;
        }
        return false;
    }

    protected abstract bool JointWithOverride(GameObject other);

    public virtual bool RemoveJoint()
    {
        m_connectedGameobject = null;
        return true;
    }

    public virtual bool Used()
    {
        return m_joint != null;
    }
}

