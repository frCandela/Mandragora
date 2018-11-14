using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationLiquidEffect : LiquidEffect
{    
    [SerializeField] private Material material;
    [SerializeField] private float duration = 10f;

    private Rigidbody m_rb = null;
    private MeshRenderer m_meshRenderer = null;
    private Color m_previousColor;

    public override void ApplyEffect(GameObject gameObject) 
    {
        if(!gameObject.GetComponent<LevitationLiquidEffect>() && gameObject.GetComponent<Rigidbody>())
        {
            LevitationLiquidEffect lle = gameObject.AddComponent<LevitationLiquidEffect>();
            lle.duration = duration;
            lle.material = material;
        }
    }

    private float m_startTime = 0f; 
    private void Start()
    {
        m_startTime = Time.time;

        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (m_meshRenderer && m_meshRenderer.material)
        {
            m_previousColor = m_meshRenderer.material.color;
            m_meshRenderer.material.color = m_meshRenderer.material.color * material.color;
        }

        m_rb = gameObject.GetComponent<Rigidbody>();
        if (m_rb)
        {
            m_rb.useGravity = false;
            m_rb.AddForce(0.05f * Vector3.up, ForceMode.Impulse);
            m_rb.AddTorque(0.05f * new Vector3(1,1,1) , ForceMode.Impulse);

        }
    }

    private void Update()
    {
        if (Time.time > m_startTime + duration)
            RemoveEffect(gameObject);
    }

    public override void RemoveEffect(GameObject go)
    {
        print("zob2");
        if (m_rb)
        {
            m_rb.useGravity = true;
        }
        if (m_meshRenderer && m_meshRenderer.material)
        {
            m_meshRenderer.material.color = m_previousColor;
        }
        Destroy(this);
    }
}
