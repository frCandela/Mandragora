using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class PlanetScalerPro : MonoBehaviour
{
    [Header("Scaling parameters")]
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;

    private DropZone m_dropzone;
    private IcoPlanet m_icoPlanet;

    private ConfigurableJoint m_confJoint;
    private ScaleEffect m_scaleEffect;
    private MTK_JointType m_scaleSphereJoint;


    [SerializeField] private float m_baseDist = -1f;
    [SerializeField] private float m_baseScale = -1f;
    [SerializeField] private float m_intermediateScale = -1f;
    Quaternion m_baseRotation;

    // Use this for initialization
    void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableScaling);

        //m_confJoint = m_scaleSphere.GetComponent<ConfigurableJoint>();
        //m_dropZone.onObjectCatched.AddListener(EnableScaling);
        //m_baseScale = m_scaleSphere.transform.localScale.x;
        // m_scaleSphere.SetActive(false);
    }


    private void Start()
    {
        //m_scaleSphereJoint = m_scaleSphere.GetComponent<MTK_JointType>();
    }
    
    void Update ()
    {
        // If the scale sphere is grabbed
       /* if(m_scaleSphereJoint.Used())
        {
            // Set reference values if not set
            if (m_baseDist == -1f)
            {
                m_baseDist = Vector3.Distance(m_scaleSphereJoint.joint.transform.position, transform.position);
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
            m_dropZone.transform.rotation = m_scaleSphere.transform.rotation;
        }
        else
        {
            m_baseDist = -1f;
        }*/
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

               /* foreach( IcoSegment segment in m_icoPlanet.Segments )
                {
                    segment.GetComponent<MTK_Interactable>().isGrabbable = true;
                }*/
            }

            /*m_scaleSphere.SetActive(true);
            m_scaleEffect = m_dropZone.catchedObject.GetComponent<ScaleEffect>();
            if (!m_scaleEffect)
            {
                m_scaleEffect = m_dropZone.catchedObject.gameObject.AddComponent<ScaleEffect>();
                m_scaleEffect.ApplyEffect();
            }*/

        }
        else
        {
           /* m_scaleSphere.SetActive(false);
            m_scaleEffect = null;*/
        }
    }
}
