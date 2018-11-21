using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class JointBreakEvent : UnityEvent<MTK_JointType>
{
}

public abstract class MTK_JointType : MonoBehaviour
{
    public JointBreakEvent onJointBreak;
    protected Joint m_joint;

    public abstract bool JoinWith(GameObject other);

    public abstract bool RemoveJoint();

    public virtual bool Used()
    {
        return m_joint != null;
    }

    protected virtual void OnJointBreak(float breakForce)
    {
        m_joint = null;
        onJointBreak.Invoke(this);
    }



}

