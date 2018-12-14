using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetEffect : Effect
{
    public Rigidbody m_rb;
    public Rigidbody sunRigidbody;

    private MTK_JointType m_joint;

    float radiusScale = 10f;    //10 20000

    /*[SerializeField]*/ private float GravitationalConstant = 0.005f;


    public float m_maxRadius = float.MaxValue;
    public bool m_radiusDecided = false;

    TelekinesisPointer[] m_pointers;

    public override bool ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb && !m_rb.isKinematic)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;
            m_joint = GetComponent<MTK_JointType>();

            m_pointers = FindObjectsOfType<TelekinesisPointer>();

            return true;
        }
        else
        {
            Destroy(this);
            return false;
        }
    }

    void ManageEffectsCollisions()
    {
        LevitationEffect levEffect = GetComponent<LevitationEffect>();
        if (levEffect)
            Destroy(levEffect);
    }

    public override void RemoveEffect()
    {
        if (m_rb)
        {
            m_rb.useGravity = true;
        }
    }

    Vector3 axis;
    float baseVel;
    private void FixedUpdate()
    {
        bool pointedAt = false;
        foreach( TelekinesisPointer pointer in m_pointers)
        {
            if(pointer.connectedBody == m_rb)
            {
                pointedAt = true;
            }
        }

        Vector3 dir = transform.position - sunRigidbody.transform.position;
        float radius = dir.magnitude;
        dir.Normalize();
        if (!m_joint.Used() && !pointedAt)
        {
            m_rb.useGravity = false;
            if (radius < m_maxRadius)
            {
                m_maxRadius = radius;
                axis = Vector3.Cross(m_rb.velocity, dir).normalized;
                baseVel = m_rb.velocity.magnitude;
            }
            else
            {
                transform.position = sunRigidbody.transform.position +  m_maxRadius * (transform.position - sunRigidbody.transform.position).normalized;
                Vector3 dir2 = (transform.position - sunRigidbody.transform.position).normalized;
                float vel = m_rb.velocity.magnitude;
                m_rb.velocity = baseVel * Vector3.Cross(dir2, axis);
            }
        }
        else
        {
            m_maxRadius = float.MaxValue;
        }
    }
}
