using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlanetScalerPro : MonoBehaviour
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
    [SerializeField] private float m_baseScale = -1f;
    [SerializeField] private float m_intermediateScale = -1f;
    Quaternion m_baseRotation;

    // Use this for initialization
    void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableScaling);
        m_confJoint = GetComponent<ConfigurableJoint>();
    }
    

    public float ratioTest;
    void Update ()
    {
        // If the scale sphere is grabbed
        if(m_dropzone.catchedObject.jointType && m_dropzone.catchedObject.jointType.Used())
        {
            print(m_dropzone.catchedObject.jointType.name);
            Debug.DrawLine(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position, Color.red);

            if (m_baseDist == -1f)
            {
                m_baseDist = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
                m_baseRotation = m_dropzone.transform.rotation;
            }

            float distance = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);
            float ratio = distance / m_baseDist;
            float scale = Mathf.Clamp(ratio * m_intermediateScale, m_minScaleRatio * m_baseScale, m_maxScaleRatio * m_baseScale);
            float realRatio = scale / m_baseScale;

            ratioTest = m_baseDist;

            // Set reference values if not set
            /* if (m_baseDist == -1f)
             {
                 m_baseDist = Vector3.Distance(m_catchedObjectJoint.joint.transform.position, transform.position);
                 m_intermediateScale = m_scaleSphere.transform.localScale.x;
                 m_baseRotation = m_dropZone.transform.rotation;
             }
             
             float distance = Vector3.Distance(m_scaleSphereJoint.joint.transform.position, transform.position);
             float ratio = distance / m_baseDist;
             float scale = Mathf.Clamp(ratio * m_intermediateScale, m_minScaleRatio * m_baseScale, m_maxScaleRatio * m_baseScale);
             float realRatio = scale / m_baseScale;

             // Scale sphere & planet
             m_scaleSphere.transform.localScale = new Vector3(scale , scale , scale );
             if (m_scaleEffect)
             {
                 m_scaleEffect.transform.localScale = realRatio * m_scaleEffect.originalScale;
             }

             //Set anchor point
             m_scaleSphereJoint.joint.connectedAnchor = m_scaleSphereJoint.joint.connectedBody.transform.InverseTransformPoint(m_scaleSphereJoint.joint.transform.position);

             // Set rotation
             m_dropZone.transform.rotation = m_scaleSphere.transform.rotation;*/
        }
        else
        {
            m_baseDist = -1f;
        }
    }
    
    void EnableScaling(bool state)
    {
        if (state)
        {
            m_icoPlanet = m_dropzone.catchedObject.GetComponent<IcoPlanet>();
            if (m_icoPlanet)
            {
                print("ICOOOOOOO");
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
            m_catchedObjectJoint = null;
            m_scaleEffect = null;
        }
    }
}
