using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshRenderer))]
public class PlanetScaler : MonoBehaviour
{
    [SerializeField, Range(0, 1f)] private float m_scalingSpeed = 1f;
    [SerializeField, Range(0, 10f)] private float m_maxScaleFactor = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleFactor = 0.5f;
    [SerializeField] private bool m_scalingUp = true;

    [SerializeField] private DropZone m_dropZone;

    private Outline m_outline;
    private ScaleEffect m_scaleEffect;
    
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
    public float ratio;
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

                ratio = scale.x / m_scaleEffect.originalScale.x;
                if(ratio > m_maxScaleFactor)
                {
                    m_scalingUp = false;
                }
                else if (ratio < m_minScaleFactor)
                {
                    m_scalingUp = true;
                }

                m_scaleEffect.transform.localScale = scale;
            }
            else
            {

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
