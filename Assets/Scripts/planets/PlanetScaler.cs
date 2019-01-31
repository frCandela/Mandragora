using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class PlanetScaler : Workshop
{
    [Header("Scaling parameters")]
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;

    private ConfigurableJoint m_confJoint;
    private ScaleEffect m_scaleEffect;
    private MTK_JointType m_catchedObjectJoint;

    private float m_baseDist = -1f;
    private float m_intermediateScale = -1f;
    private float m_newScale = -1f;

    private void Update()
    {
        if(m_newScale > 0f)
        {
            m_scaleEffect.transform.localScale = new Vector3(m_newScale, m_newScale, m_newScale);
            m_newScale = -1f;
        }
    }



    void FixedUpdate ()
    {
        // If the scale sphere is grabbed
        if(m_dropzone.catchedObject && m_dropzone.catchedObject.jointType &&
            m_dropzone.catchedObject.jointType.Used())
        {
            if (m_baseDist == -1f)
            {
                m_baseDist = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
                m_intermediateScale = m_scaleEffect.transform.localScale.x;

                m_confJoint = m_catchedObjectJoint.connectedGameobject.gameObject.AddComponent<ConfigurableJoint>();
                m_confJoint.connectedBody = m_catchedObjectJoint.GetComponent<Rigidbody>();

                m_confJoint.autoConfigureConnectedAnchor = false;
                m_confJoint.xMotion = ConfigurableJointMotion.Locked;
                m_confJoint.yMotion = ConfigurableJointMotion.Locked;
                m_confJoint.zMotion = ConfigurableJointMotion.Locked;
            }

            float distance = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
            float ratio = distance / m_baseDist;
            m_newScale = Mathf.Clamp(ratio * m_intermediateScale, m_minScaleRatio * m_scaleEffect.originalScale.x, m_maxScaleRatio * m_scaleEffect.originalScale.x);
            
            

            Vector3 anchorPoint = m_confJoint.connectedBody.transform.TransformPoint(m_confJoint.connectedAnchor);
            Vector3 dir = anchorPoint - m_confJoint.connectedBody.transform.position;

            dir = distance * dir.normalized;
            m_confJoint.connectedAnchor = m_confJoint.connectedBody.transform.InverseTransformPoint(m_confJoint.connectedBody.transform.position + dir);           
        }
        else
        {
            ResetHand();
        }
    }

    void ResetHand()
    {
        m_baseDist = -1f;
        Destroy(m_confJoint);
    }
    
    protected override void OnWorkshopUpdateState(bool state, MTK_Interactable current)
    {
        if (state)
        {
            m_catchedObjectJoint = current.jointType;
            current.jointType.onJointBreak.AddListener(ResetHand);

            m_scaleEffect = current.GetComponent<ScaleEffect>();

            if (!m_scaleEffect)
            {
                m_scaleEffect = current.gameObject.AddComponent<ScaleEffect>();
                m_scaleEffect.ApplyEffect();
            }
        }
        else
        {
            m_catchedObjectJoint = null;
            m_scaleEffect = null;
        }
    }
}
