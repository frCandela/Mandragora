using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class PlanetScaler : Workshop
{
    [Header("Scaling parameters")]
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;

    private ScaleEffect m_scaleEffect;

    MTK_InputManager m_currentInputmanager;

    private float m_baseDist = -1f;
    private float m_intermediateScale = -1f;
    private float m_newScale = -1f;
    private float m_oldScale = -1f;

    private void Update()
    {
        if(m_newScale > 0f)
        {
            m_scaleEffect.transform.localScale = Vector3.one * m_newScale;
            
            if(!m_currentInputmanager)
                m_currentInputmanager = m_catchedObjectJoint.GetComponent<MTK_InertJoint>().connectedGameobject.GetComponentInParent<MTK_InputManager>();
            else
                m_currentInputmanager.Haptic(Mathf.Abs(m_oldScale - m_newScale) * 10);

            m_oldScale = m_newScale;
            m_newScale = -1f;
        }
    }

    float m_oldDistance;
    protected override void OnObjectGrabStay()
    {
        float distance = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
        float ratio = distance / m_baseDist;
        m_newScale = Mathf.Clamp(ratio * m_intermediateScale, m_minScaleRatio * m_scaleEffect.originalScale.x, m_maxScaleRatio * m_scaleEffect.originalScale.x);
        
        AkSoundEngine.SetRTPCValue("Scale_Rate", Mathf.Abs(distance - m_oldDistance) * 10000);
        
        m_oldDistance = distance;
    }

    protected override void OnObjectGrabStart()
    {
        m_baseDist = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
        m_intermediateScale = m_scaleEffect.transform.localScale.x;
    }
    
    protected override void OnWorkshopUpdateState(bool state, MTK_Interactable current)
    {
        if (state)
        {
            m_scaleEffect = current.GetComponent<ScaleEffect>();

            if (!m_scaleEffect)
            {
                m_scaleEffect = current.gameObject.AddComponent<ScaleEffect>();
                m_scaleEffect.ApplyEffect();
            }

            AkSoundEngine.PostEvent("LFO_Scale_Play", gameObject);
        }
        else
        {
            if(m_scaleEffect)
                m_scaleEffect.RemoveEffect();
                
            m_scaleEffect = null;

            AkSoundEngine.PostEvent("LFO_Scale_Stop", gameObject);
        }
    }

    protected override IEnumerator AnimateWorkshop(float duration, VoidDelegate onFinish)
    {
        float scaleIncrement = m_scaleEffect.transform.localScale.x / 2;
        Vector3 initScale = m_scaleEffect.transform.localScale;

        for (float t = 0; t < 2; t += Time.deltaTime / duration)
        {
            m_scaleEffect.transform.localScale = initScale + initScale.normalized * Mathf.Sin(t * Mathf.PI) * scaleIncrement;

            yield return new WaitForEndOfFrame();
        }
        
        onFinish.Invoke();
    }
}