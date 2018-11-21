using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityEffect : Effect
{
    private Rigidbody m_rb;

    public override void ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb)
        {
            ManageEffectsCollisions();
            m_rb.useGravity = false;
        }
        else
            Destroy(this);
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
