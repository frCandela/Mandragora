using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScalerPro : MonoBehaviour
{
    [Header("Scaling parameters")]
    [SerializeField, Range(0, 10f)] private float m_maxScaleRatio = 2f;
    [SerializeField, Range(0, 10f)] private float m_minScaleRatio = 0.5f;

    [Header("References")]
    [SerializeField] private DropZone m_dropZone;
    [SerializeField] private GameObject scaleSphere;

    private ConfigurableJoint m_confJoint;
    private ScaleEffect m_scaleEffect;
    private MTK_JointType m_scaleSphereJoint;
    [SerializeField] private float m_baseDist = -1f;
    [SerializeField] private float m_baseScale = -1f;

    // Use this for initialization
    void Awake ()
    {
        m_confJoint = scaleSphere.GetComponent<ConfigurableJoint>();

        m_dropZone.onObjectCatched.AddListener(EnableScaling);
    }


    private void Start()
    {
        m_scaleSphereJoint = scaleSphere.GetComponent<MTK_JointType>();
    }

    public float test;
    void Update ()
    {
        if(m_scaleSphereJoint.Used())
        {
            if (m_baseDist == -1f)
            {
                m_baseDist = Vector3.Distance(m_scaleSphereJoint.joint.transform.position, transform.position);
                m_baseScale = scaleSphere.transform.localScale.x;
            }

            float distance = Vector3.Distance(m_scaleSphereJoint.joint.transform.position, transform.position);
            test = distance / m_baseDist;

            scaleSphere.transform.localScale = new Vector3(test * m_baseScale, test * m_baseScale, test * m_baseScale);

            Debug.DrawLine(m_scaleSphereJoint.joint.transform.position, m_scaleSphereJoint.joint.connectedBody.transform.position + m_scaleSphereJoint.joint.connectedAnchor);
        }
        else
        {
            m_baseDist = -1f;
        }


        /*float distance = Vector3.Distance(scaleSphere.transform.position, transform.position);

        float ratio = (Mathf.Clamp((distance - m_baseDist) / m_confJoint.linearLimit.limit, -1, 1) + 1 ) / 2; // between 0 and 1
        float targetRatio= (m_maxScaleRatio - m_minScaleRatio) * ratio + m_minScaleRatio;

        if (m_scaleSphereJoint.joint && Mathf.Abs(distance - m_baseDist) > m_confJoint.linearLimit.limit)
        {
            MTK_InputManager inputManager = m_scaleSphereJoint.joint.GetComponent<MTK_InteractHand>().inputManager;
            inputManager.Haptic(1f);
        }
        if (m_scaleEffect)
        {
            m_scaleEffect.transform.localScale = targetRatio * m_scaleEffect.originalScale;


        }*/
    }
    
    void EnableScaling(bool state)
    {
        if (state)
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
}
