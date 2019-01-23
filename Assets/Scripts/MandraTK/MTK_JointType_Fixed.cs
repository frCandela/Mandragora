using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_JointType_Fixed : MTK_JointType
{
    public float breakForce = 1500f;

    protected virtual void Awake()
    {
        if( !rigidbody)
        {
            rigidbody = GetComponent<Rigidbody>();
            if( ! rigidbody)
                rigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }

    protected override bool JointWithOverride(GameObject other)
    {
        if (!rigidbody)
        {
            ObjectOnPlanet oop = GetComponent<ObjectOnPlanet>();
            if( oop )
                oop.Dissociate();
            return false;
        }

        if ( ! m_joint )
        {
            rigidbody.velocity = Vector3.zero; // Prevent joint break
            m_joint = other.AddComponent<FixedJoint>();
            m_joint.connectedBody = rigidbody;
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