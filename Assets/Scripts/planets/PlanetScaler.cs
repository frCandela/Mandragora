using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshRenderer))]
public class PlanetScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DropZone m_dropZone;

    [Header("Scaling parameters")]
    [SerializeField, Range(0, 1f)] private float m_scalingSpeed = 0.3f;
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;  


    private Outline m_outline;
    private ScaleEffect m_scaleEffect;
    private bool m_scalingUp = true;
    private int m_nbHandsInTrigger = 0;

    // Use this for initialization
    void Awake ()
    {
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;
        m_dropZone.onObjectCatched.AddListener(EnableScaling);
        EnableScaling(false);
    }

    void EnableScaling( bool state )
    {
        GetComponent<MeshRenderer>().enabled = state;

        if(state)
        {
            m_scaleEffect = m_dropZone.catchedObject.GetComponent<ScaleEffect>();
            if (!m_scaleEffect)
            {
                m_scaleEffect = m_dropZone.catchedObject.gameObject.AddComponent<ScaleEffect>();
                m_scaleEffect.ApplyEffect();
            }
        }
        else
        {
            m_scaleEffect = null;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if(m_scaleEffect)
        {
            if (m_nbHandsInTrigger > 0)
            {
                Vector3 scale = m_scaleEffect.transform.localScale;
                float deltaScale = m_scalingSpeed * Time.deltaTime;
                if (m_scalingUp)
                {
                    scale += new Vector3(deltaScale, deltaScale, deltaScale);
                }
                else
                {
                    scale -= new Vector3(deltaScale, deltaScale, deltaScale);
                }

                float ratio = scale.x / m_scaleEffect.originalScale.x;
                if(ratio > m_maxScaleRatio)
                {
                    m_scalingUp = false;
                }
                else if (ratio < m_minScaleRatio)
                {
                    m_scalingUp = true;
                }

                m_scaleEffect.transform.localScale = scale;
            }
        }
    }

    void UpdateOutline()
    {
        if (m_nbHandsInTrigger > 0)
        {
            m_outline.enabled = true;
        }
        else
        {
            m_outline.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MTK_InteractHand>())
        {
            m_nbHandsInTrigger++;
            UpdateOutline();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MTK_InteractHand>())
        {
            m_nbHandsInTrigger--;
            UpdateOutline();
        }
    }
}
