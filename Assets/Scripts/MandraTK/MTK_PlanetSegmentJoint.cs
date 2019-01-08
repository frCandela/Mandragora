using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IcoSegment))]
public class MTK_PlanetSegmentJoint : MTK_JointType
{
    public bool m_editMode = false;

    private IcoPlanet m_icoPlanet;
    private IcoSegment m_icoSegment;
    private Rigidbody m_rigidbody;

    private float m_baseDistance = 0f;

    protected override void Awake()
    {
        base.Awake();

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
            return base.JointWithOverride(other);
        }
        else
        {
            if (!Used())
            {
                m_baseDistance = Vector3.Distance(transform.position, other.transform.position);
                return true;
            }
        }
        
        return false;
    }

    public override bool RemoveJoint()
    {
        if (m_editMode)            
        {
            return Used();
        }
        else
        {
            return base.RemoveJoint();
        }

    }


    private void Update()
    {
        if (Used())
        {
            float delta = Vector3.Distance(transform.position, m_connectedGameobject.transform.position) - m_baseDistance;
            int heightSteps = (int)( delta / (m_icoPlanet.heightDelta * m_icoPlanet.transform.localScale.x) );

            if(heightSteps != 0)
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