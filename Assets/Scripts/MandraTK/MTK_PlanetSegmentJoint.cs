using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IcoSegment))]
public class MTK_PlanetSegmentJoint : MTK_JointType
{
    [SerializeField] private float breakForce = 1500f;

    public bool m_editMode = false;

    private IcoPlanet m_icoPlanet;
    private IcoSegment m_icoSegment;

    private float m_baseDistance = 0f;

    protected void Awake()
    {
        rigidbody = transform.parent.GetComponent<Rigidbody>();
        m_icoPlanet = transform.parent.GetComponent<IcoPlanet>();
        m_icoSegment = GetComponent<IcoSegment>();
    }

    public override bool Used()
    {
        return m_connectedGameobject;
    }

    protected override bool JointWithOverride(GameObject other)
    {
        if (m_editMode)
        {
            if (!Used())
            {
                m_baseDistance = Vector3.Distance(transform.position, other.transform.position);
                return true;
            }
        }
        else
        {
            if (!m_joint)
            {
                rigidbody.velocity = Vector3.zero; // Prevent joint break
                m_joint = other.AddComponent<FixedJoint>();
                m_joint.connectedBody = rigidbody;
                m_joint.breakForce = breakForce;
                return true;
            }
            return false;
        }
        
        return false;
    }

    public override bool RemoveJoint()
    {
        base.RemoveJoint();
        if (m_editMode)            
        {
            return Used();
        }
        else
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


    private void Update()
    {
        if (m_editMode)
        {
            if (Used())
            {
                float delta = Vector3.Distance(transform.position, m_connectedGameobject.transform.position) - m_baseDistance;
                int heightSteps = (int)(delta / (m_icoPlanet.heightDelta * m_icoPlanet.transform.localScale.x));

                if (heightSteps != 0)
                {
                    m_icoSegment.heightLevel += heightSteps;
                    m_baseDistance += heightSteps * (m_icoPlanet.heightDelta * m_icoPlanet.transform.localScale.x);
                    m_icoSegment.UpdateSegment();
                    m_icoSegment.UpdateNeighbours();
                    print(m_baseDistance + " " + heightSteps);
                }

                Debug.DrawLine(transform.position, m_connectedGameobject.transform.position, Color.red);
            }
        }
    }
}