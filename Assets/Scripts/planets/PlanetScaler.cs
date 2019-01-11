using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class PlanetScaler : MonoBehaviour
{
    [Header("Scaling parameters")]
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;

    private DropZone m_dropzone;
    private IcoPlanet m_icoPlanet;
    private ConfigurableJoint m_confJoint;
    private ScaleEffect m_scaleEffect;
    private MTK_JointType m_catchedObjectJoint;

    [SerializeField] private float m_baseDist = -1f;
    [SerializeField] private float m_intermediateScale = -1f;

    // Use this for initialization
    void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableScaling);
    }

    public bool test = false;

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
            float scale = Mathf.Clamp(ratio * m_intermediateScale, m_minScaleRatio * m_scaleEffect.originalScale.x, m_maxScaleRatio * m_scaleEffect.originalScale.x);
            
            m_scaleEffect.transform.localScale = new Vector3(scale, scale, scale);

            Vector3 anchorPoint = m_confJoint.connectedBody.transform.TransformPoint(m_confJoint.connectedAnchor);
            Vector3 dir = anchorPoint - m_confJoint.connectedBody.transform.position;

            dir = distance * dir.normalized;
            m_confJoint.connectedAnchor = m_confJoint.connectedBody.transform.InverseTransformPoint(m_confJoint.connectedBody.transform.position + dir);           
        }
        else
        {
            m_baseDist = -1f;
            Destroy(m_confJoint);
        }
    }
    
    void EnableScaling(bool state)
    {
        if (state)
        {
            m_icoPlanet = m_dropzone.catchedObject.GetComponent<IcoPlanet>();
            if (m_icoPlanet)
            {
                MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();
                interactable.isDistanceGrabbable = false;
                interactable.IndexJointUsed = 1;

                m_catchedObjectJoint = interactable.jointType;

                m_scaleEffect = m_dropzone.catchedObject.GetComponent<ScaleEffect>();

                if (!m_scaleEffect)
                {
                    m_scaleEffect = m_dropzone.catchedObject.gameObject.AddComponent<ScaleEffect>();
                    m_scaleEffect.ApplyEffect();
                }
            } 
        }
        else
        {
            MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();
            interactable.IndexJointUsed = 0;
            interactable.isDistanceGrabbable = true;

            m_catchedObjectJoint = null;
            m_scaleEffect = null;
        }
    }
}
