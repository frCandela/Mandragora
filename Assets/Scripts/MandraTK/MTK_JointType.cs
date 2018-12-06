using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MTK_JointType : MonoBehaviour
{
    public UnityEvent onJointBreak = new UnityEvent();

    [SerializeField] protected Joint m_joint;

    public abstract bool JoinWith(GameObject other);

    public abstract bool RemoveJoint();

    public virtual bool Used()
    {
        return m_joint != null;
    }
}

