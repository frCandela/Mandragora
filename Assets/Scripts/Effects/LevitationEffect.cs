using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationEffect : Effect
{
    private Rigidbody m_rb;    

    public override bool ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb && !m_rb.isKinematic)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;
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
        NoGravityEffect noGravEffect = GetComponent<NoGravityEffect>();
        if (noGravEffect)
            Destroy(noGravEffect);
    }

    public override void RemoveEffect()
    {
        if (m_rb)
        {
            m_rb.useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        m_rb.AddForce(-Physics.gravity);
    }
}
