using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Fixed : MTK_JointType
{
    public float breakForce = 1500f;

    private FixedJoint m_joint;
    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public override bool JoinWith(GameObject other)
    {
        m_joint = other.AddComponent<FixedJoint>();
        m_joint.connectedBody = m_rb;
        m_joint.breakForce = breakForce;
        return true;
    }

    public override bool RemoveJoinWith(GameObject other)
    {
        Destroy(m_joint);
        m_joint = null;
        return true;
    }
}