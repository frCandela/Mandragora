using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Rigidbody))]
public class MTK_JointType_Track : MTK_JointType
{
    [SerializeField] private float dragScale = 500;
    [SerializeField] private float force = 0.05f;

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
            Vector3 dir = (m_connectedBody.transform.position- applicationPoint.transform.position);

            m_rb.AddForceAtPosition(force * dir, applicationPoint.transform.position, ForceMode.VelocityChange);
            dir.Normalize();


            // Removes velocity in the wrong direction
            float projVel = Vector3.Dot(m_rb.velocity, dir);
             if (projVel < 0)
             {
                 m_rb.drag = Mathf.Min( 100f, m_rb.drag + Time.fixedDeltaTime * dragScale);
             }
             else
                 m_rb.drag = 1f;



             Vector3 rbToAppPoint = applicationPoint.transform.position - m_rb.transform.position;
             Vector3 AppPointToConnexedBdy = m_connectedBody.transform.position - applicationPoint.transform.position;
             Vector3 desiredAxis = Vector3.Cross(rbToAppPoint, AppPointToConnexedBdy).normalized;

             float projRot = Vector3.Dot(m_rb.angularVelocity, desiredAxis);
             if(projRot < 0)
             {
                 m_rb.angularDrag = Mathf.Min( 100f, m_rb.angularDrag + Time.fixedDeltaTime * dragScale);
             }
             else
                 m_rb.angularDrag = 1f;
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
 