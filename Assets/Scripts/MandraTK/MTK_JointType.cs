using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MTK_JointType : MonoBehaviour
{
    public UnityEvent onJointBreak = new UnityEvent();

    [SerializeField] protected Joint m_joint;
    [SerializeField] protected GameObject m_connectedGameobject;

    public Joint joint { get { return m_joint; } }
    public GameObject connectedGameobject { get { return m_connectedGameobject; } }

    public virtual bool JoinWith(GameObject other)
    {
        m_connectedGameobject = other;
        return true;
    }


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

