using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationLiquidEffect : LiquidEffect
{    
    [SerializeField] private Material material;
    [SerializeField] private float duration = 10f;
    [SerializeField] private float removeDuration = 2f;

    private Rigidbody m_rb = null;
    private MeshRenderer m_meshRenderer = null;
    private Material m_previousMat;
    private float m_startTime = 0f;

    public override void ApplyEffect(GameObject gameObject) 
    {
        if(!gameObject.GetComponent<LevitationLiquidEffect>() && gameObject.GetComponent<Rigidbody>())
        {
            LevitationLiquidEffect lle = gameObject.AddComponent<LevitationLiquidEffect>();
            lle.duration = duration;
            lle.material = material;
        }
    }


    private void Start()
    {
        m_startTime = Time.time;

        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (m_meshRenderer && m_meshRenderer.material)
        {
            m_previousMat = m_meshRenderer.material;
            m_meshRenderer.material = material;
        }

        m_rb = gameObject.GetComponent<Rigidbody>();
        if (m_rb)
        {
            m_rb.useGravity = false;            

            m_rb.AddForce(0.05f * Vector3.up, ForceMode.Impulse);
            m_rb.AddTorque(0.05f * new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized , ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (Time.time > m_startTime + duration)
        {
            float scale = (Time.time - (m_startTime + duration)) / ( duration + removeDuration);
            m_rb.AddForce(scale * Physics.gravity);

            if (Time.time > m_startTime + duration + removeDuration)
                RemoveEffect(gameObject);
        }
    }

    public override void RemoveEffect(GameObject go)
    {
        if (m_rb)
        {
            m_rb.useGravity = true;
        }
        if (m_meshRenderer && m_meshRenderer.material)
        {
            m_meshRenderer.material = m_previousMat;
        }
        Destroy(this);
    }
}
