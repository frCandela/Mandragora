using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlanetEffect : Effect
{
    public Rigidbody m_rb;
    public Rigidbody sunRigidbody;
    public GameObject explosionEffect;


    private MTK_JointType m_joint;

    public float m_maxRadius;

    public float maxSpeed = float.PositiveInfinity;
    public float accelerationForce = 1;
    public float impactForce = 3f;

    public bool m_radiusDecided = false;
    public bool effectActive = true;
    public override bool ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        AkSoundEngine.PostEvent("Planet_Rotation_Play", gameObject);

        m_maxRadius = float.MaxValue;
        
        if(m_lastVel != Vector3.zero)
            m_rb.velocity = m_lastVel;

        if (m_rb && !m_rb.isKinematic)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;
            m_joint = GetComponent<MTK_JointType>();

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
            m_lastVel = m_rb.velocity;
            m_rb.useGravity = true;
        }
        AkSoundEngine.PostEvent("Planet_Rotation_Stop", gameObject);
    }

    Vector3 axis;
    float baseVel;
    Vector3 m_lastVel;

    private void FixedUpdate()
    {
        if (!m_joint.Used()&& effectActive)
        {
            Vector3 dir = transform.position - sunRigidbody.transform.position;
            float radius = dir.magnitude;
            dir.Normalize();
            if (baseVel > maxSpeed)
            {
               // float delta = (baseVel - maxSpeed) * Time.fixedDeltaTime / accelerationDuration;
                baseVel -= accelerationForce * Time.fixedDeltaTime;
            }

            if (radius < m_maxRadius)
            {
                m_maxRadius = radius;
                axis = Vector3.Cross(m_rb.velocity, dir).normalized;
                baseVel = m_rb.velocity.magnitude;
            }
            else
            {
                m_rb.position = sunRigidbody.transform.position + m_maxRadius * (transform.position - sunRigidbody.position).normalized;
                Vector3 dir2 = (transform.position - sunRigidbody.transform.position).normalized;
                m_rb.velocity = baseVel * Vector3.Cross(dir2, axis);
            }
        }
        else
        {
            m_maxRadius = float.MaxValue;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if( ! m_rb.useGravity)
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            m_rb.AddForce(impactForce * dir, ForceMode.Impulse);
            Destroy(Instantiate(explosionEffect, transform.position, transform.rotation), 10);
        }            
    }
}
