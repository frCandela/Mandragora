using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Fixed : MTK_JointType
{
    [SerializeField] private Rigidbody m_rigidbody;

    public float breakForce = 1500f;

    private void Awake()
    {
        if( !m_rigidbody )
            m_rigidbody = GetComponent<Rigidbody>();
    }
    public override bool JoinWith(GameObject other)
    {
        if( ! m_joint )
        {
            m_joint = other.AddComponent<FixedJoint>();
            m_joint.connectedBody = m_rigidbody;
            m_joint.breakForce = breakForce;
            return true;
        }
        return false;
    }

    public override bool RemoveJoint()
    {
        if(m_joint)
        {
            onJointBreak.Invoke();
            Destroy(m_joint);
            m_joint = null;
            return true;
        }
        return false;
    }
}