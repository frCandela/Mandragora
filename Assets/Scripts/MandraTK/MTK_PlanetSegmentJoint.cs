using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_PlanetSegmentJoint : MTK_JointType
{
    public bool m_editMode = false;

    private IcoPlanet m_icoPlanet;
    private IcoSegment m_icoSegment;

    private float m_baseDistance = 0f;
    private Vector3 m_basePosition;
    int m_oldHeight;

    // Planet rotation
    private bool m_grabbing = false;
    private ConfigurableJoint m_confJoint;

    protected void Awake()
    {
        rigidbody = transform.parent.GetComponent<Rigidbody>();
        m_icoPlanet = transform.parent.GetComponent<IcoPlanet>();
        m_icoSegment = GetComponent<IcoSegment>();

        m_oldHeight = m_icoSegment.heightLevel;
    }

    public override bool Used()
    {
        return m_connectedGameobject;
    }

    public override bool RemoveJoint()
    {
        m_grabbing = false;
        Destroy(m_confJoint);

        return base.RemoveJoint();
    }

    protected override bool JointWithOverride(GameObject other)
    {
        if( !Used())
        {
            m_baseDistance = Vector3.Distance(transform.position, other.transform.position);
            m_basePosition = transform.parent.position;
            m_confJoint = other.AddComponent<ConfigurableJoint>();
            m_confJoint.connectedBody = transform.parent.GetComponent<Rigidbody>();

            m_confJoint.autoConfigureConnectedAnchor = false;
            m_confJoint.xMotion = ConfigurableJointMotion.Locked;
            m_confJoint.yMotion = ConfigurableJointMotion.Locked;
            m_confJoint.zMotion = ConfigurableJointMotion.Locked;

            m_confJoint.enableCollision = true;

            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (Used())
        {
            // Set height segment
            float delta = Vector3.Distance(transform.position, m_connectedGameobject.transform.position) - m_baseDistance;
            int heightSteps = (int)(delta / (m_icoPlanet.heightDelta * m_icoPlanet.transform.localScale.x));

            if (heightSteps != 0)
            {
                m_icoSegment.heightLevel += heightSteps;
                m_baseDistance += heightSteps * (m_icoPlanet.heightDelta * m_icoPlanet.transform.localScale.x);
                m_icoSegment.UpdateSegment();
                m_icoSegment.UpdateNeighbours();

                if(m_oldHeight != m_icoSegment.heightLevel)
                {
                    if(m_icoSegment.heightLevel  <= 0)
                        AkSoundEngine.PostEvent("Water_Play", gameObject);
                    else if (heightSteps  > 0)
                        AkSoundEngine.PostEvent("Stone_Up_Play", gameObject);
                    else if (heightSteps < 0)
                        AkSoundEngine.PostEvent("Stone_Down_Play", gameObject);
                }
                
                m_oldHeight = m_icoSegment.heightLevel;
            }

            // Set configurable joint
            
            float distance = Vector3.Distance(m_basePosition, m_confJoint.transform.position);
            Vector3 anchorPoint = m_confJoint.connectedBody.transform.TransformPoint(m_confJoint.connectedAnchor);
            Vector3 dir = anchorPoint - m_confJoint.connectedBody.transform.position;

            dir = distance * dir.normalized;
            m_confJoint.connectedAnchor = m_confJoint.connectedBody.transform.InverseTransformPoint(m_confJoint.connectedBody.transform.position + dir);

            Debug.DrawLine(m_basePosition, m_basePosition + dir, Color.red);
            Debug.DrawLine(m_basePosition, m_basePosition + dir, Color.red);

            /* Vector3 dir = anchorPoint - m_confJoint.connectedBody.transform.position;

             dir = distance * dir.normalized;
             m_confJoint.connectedAnchor = m_confJoint.connectedBody.transform.InverseTransformPoint(m_confJoint.connectedBody.transform.position + dir);
           */
            //Debug.DrawLine(transform.position, m_connectedGameobject.transform.position, Color.red);
        }
    }
}