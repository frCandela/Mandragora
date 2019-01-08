using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Fixed : MTK_JointType
{
    [SerializeField] private Rigidbody m_rigidbody;

    public float breakForce = 1500f;

    protected virtual void Awake()
    {
        if( !m_rigidbody )
            m_rigidbody = GetComponent<Rigidbody>();
    }

    protected override bool JointWithOverride(GameObject other)
    {
        if( ! m_joint )
        {
            m_rigidbody.velocity = Vector3.zero; // Prevent joint break
            m_joint = other.AddComponent<FixedJoint>();
            m_joint.connectedBody = m_rigidbody;
            m_joint.breakForce = breakForce;
            return true;
        }
        return false;
    }

    public override bool RemoveJoint()
    {
        base.RemoveJoint();
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