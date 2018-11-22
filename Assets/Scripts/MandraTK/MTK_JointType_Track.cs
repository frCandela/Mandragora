using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class MTK_JointType_Track : MTK_JointType
{
    public float breakForce = 10000f;

    public override bool JoinWith(GameObject other)
    {
        if (!m_joint)
        {
            ConfigurableJoint joint = other.gameObject.AddComponent<ConfigurableJoint>();
            m_joint = joint;
            joint.connectedBody = GetComponent<Rigidbody>();
            joint.breakForce = breakForce;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            return true;
        }
        return false;
    }

    public override bool RemoveJoint()
    {
        if (m_joint)
        {
            onJointBreak.Invoke();
            Destroy(m_joint);
            m_joint = null;
            return true;
        }
        return false;
    }
}