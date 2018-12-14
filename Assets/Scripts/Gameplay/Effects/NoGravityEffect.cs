using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityEffect : Effect
{
    private Rigidbody m_rb;

    public override bool ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb && !m_rb.isKinematic)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;

            m_rb.AddForce(0.3f * Vector3.up, ForceMode.Impulse);
            m_rb.AddTorque(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized);

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

}
