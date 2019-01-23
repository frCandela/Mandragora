using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    private DropZone m_dropzone;
    private ConfigurableJoint m_confJoint;
    private MTK_Interactable m_interactable;

    [SerializeField] private MTK_JointType m_catchedObjectJoint;

    private float m_baseDist = -1f;

    void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableScaling);
    }

    void ResetHand()
    {
        m_baseDist = -1f;
        Destroy(m_confJoint);
    }

    void FixedUpdate()
    {
        // If the scale sphere is grabbed
        if (m_dropzone.catchedObject && m_dropzone.catchedObject.jointType &&
            m_dropzone.catchedObject.jointType.Used())
        {
            if (m_baseDist == -1f)
            {
                m_baseDist = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);

                m_confJoint = m_catchedObjectJoint.connectedGameobject.gameObject.AddComponent<ConfigurableJoint>();
                m_confJoint.connectedBody = m_catchedObjectJoint.GetComponent<Rigidbody>();

                m_confJoint.autoConfigureConnectedAnchor = false;
                m_confJoint.xMotion = ConfigurableJointMotion.Locked;
                m_confJoint.yMotion = ConfigurableJointMotion.Locked;
                m_confJoint.zMotion = ConfigurableJointMotion.Locked;
            }

            float distance = Vector3.Distance(transform.position, m_catchedObjectJoint.connectedGameobject.transform.position);

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

    void EnableScaling(bool state)
    {
        if (state)
        {
            m_interactable = m_dropzone.catchedObject.GetComponent<MTK_Interactable>();
            if (m_interactable)
            {
                m_interactable.isDistanceGrabbable = false;
                m_interactable.IndexJointUsed = 1;
                m_catchedObjectJoint = m_interactable.jointType;
                m_interactable.jointType.onJointBreak.AddListener(ResetHand);
            }
        }
        else
        {
            m_interactable.jointType.onJointBreak.RemoveListener(ResetHand);
            m_interactable.IndexJointUsed = 0;
            m_interactable.isDistanceGrabbable = true;

            m_interactable = null;
            m_catchedObjectJoint = null;
        }
    }
}
