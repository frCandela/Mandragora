using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationEffect : Effect
{
    private Rigidbody m_rb;

    public override void ApplyEffect()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb)
        {
            m_rb.useGravity = false;
        }
        else
            Destroy(this);
    }

    private void FixedUpdate()
    {
        m_rb.AddForce( - Physics.gravity);
    }

    public override void RemoveEffect()
    {
        if (m_rb)
        {
            m_rb.useGravity = true;
        }
    }

}
