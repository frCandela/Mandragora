using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetEffect : Effect
{
    public Rigidbody m_rb;

    public Rigidbody sunRigidbody;

    /*[SerializeField]*/ private float GravitationalConstant = 0.005f;


    public override bool ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb && !m_rb.isKinematic)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;
            print("ApplyEffect");
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
            print("RemoveEffect");
            m_rb.useGravity = true;
        }
    }


    private void FixedUpdate()
    {
        m_rb.useGravity = false;
        Vector3 dir = (sunRigidbody.transform.position - transform.position);
        float distance = dir.magnitude;
        float force = sunRigidbody.mass * m_rb.mass * GravitationalConstant;
        Vector3 forceVec = dir.normalized * force / (distance* distance);
        m_rb.AddForce(forceVec);

        Debug.DrawLine(transform.position, transform.position + forceVec, Color.red);
    }

}
