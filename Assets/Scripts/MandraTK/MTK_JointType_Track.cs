using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Track : MTK_JointType
{
    public float force = 10f;

    private Rigidbody m_rb;
    private Rigidbody m_connectedBody;

    private GameObject applicationPoint = null;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(Used())
        {
            Vector3 dir = (m_connectedBody.transform.position- applicationPoint.transform.position).normalized;
            Debug.DrawLine(applicationPoint.transform.position, applicationPoint.transform.position + dir);            
            m_rb.AddForceAtPosition(force * dir, applicationPoint.transform.position );

            // Removes velocity in the wrong direction
            float proj = Vector3.Dot(m_rb.velocity, dir);
            if (proj < 0)
                m_rb.velocity = m_rb.velocity - proj * dir;

        }
    }

    public override bool Used()
    {
        return applicationPoint != null;
    }

    public override bool JoinWith(GameObject other)
    {
        if( ! Used())
        {
            m_connectedBody = other.GetComponent<Rigidbody>();
            applicationPoint = new GameObject();
            applicationPoint.transform.position = m_connectedBody.transform.position;
            applicationPoint.transform.parent = transform;
            return true;
        }
        return false;
    }

    public override bool RemoveJoint()
    {
        if(Used())
        {
            Destroy(applicationPoint);
            onJointBreak.Invoke(this);
            return true;
        }
        return false;
    }
}