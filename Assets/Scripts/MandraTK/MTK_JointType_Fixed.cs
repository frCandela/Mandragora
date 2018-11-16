using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Fixed : MTK_JointType
{
    public float breakForce = 1500f;
    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public override bool JoinWith(GameObject other)
    {
        if( ! m_joint )
        {
            m_joint = gameObject.AddComponent<FixedJoint>();
            m_joint.connectedBody = other.GetComponent<Rigidbody>();
            m_joint.breakForce = breakForce;
            return true;
        }
        return false;
    }

    public override bool RemoveJoint()
    {
        if(m_joint)
        {
            Destroy(m_joint);
            m_joint = null;
            onJointBreak.Invoke(this);
            return true;
        }
        return false;
    }
}