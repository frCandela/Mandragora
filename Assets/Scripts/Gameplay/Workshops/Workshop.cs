using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public abstract class Workshop : MonoBehaviour
{
	protected DropZone m_dropzone;

	private ConfigurableJoint m_confJoint;
	protected MTK_JointType m_catchedObjectJoint;

	void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();

		if(!m_dropzone)
			m_dropzone = GetComponentInParent<DropZone>();
		
        m_dropzone.onObjectCatched.AddListener(EnableWorkshop);
    }

	private void FixedUpdate()
	{// If the scale sphere is grabbed
        if(m_dropzone.catchedObject && m_dropzone.catchedObject.jointType &&
           m_dropzone.catchedObject.jointType.Used())
        {
            if (!m_confJoint)
            {
				m_confJoint = m_catchedObjectJoint.connectedGameobject.gameObject.AddComponent<ConfigurableJoint>();
				m_confJoint.connectedBody = m_catchedObjectJoint.GetComponent<Rigidbody>();

				m_confJoint.autoConfigureConnectedAnchor = false;
				m_confJoint.xMotion = ConfigurableJointMotion.Locked;
				m_confJoint.yMotion = ConfigurableJointMotion.Locked;
				m_confJoint.zMotion = ConfigurableJointMotion.Locked;

				OnObjectGrabStart();
            }
			
            Vector3 anchorPoint = m_confJoint.connectedBody.transform.TransformPoint(m_confJoint.connectedAnchor);
            Vector3 dir = anchorPoint - m_confJoint.connectedBody.transform.position;

            m_confJoint.connectedAnchor = m_confJoint.connectedBody.transform.InverseTransformPoint(m_confJoint.connectedBody.transform.position + dir);

			OnObjectGrabStay();		
        }
        else
        {
            Destroy(m_confJoint);
        }
	}

    protected void EnableWorkshop(bool state)
	{
		MTK_Interactable current = m_dropzone.catchedObject;

		OnWorkshopUpdateState(state, current);

		if(state)
		{
			current.isDistanceGrabbable = false;
            current.IndexJointUsed = 1;

			current.jointType.onJointBreak.AddListener(() => Destroy(m_confJoint));
            current.isGrabbable = false;

			m_catchedObjectJoint = current.jointType;

			StartCoroutine(AnimateWorkshop(2, () => 
            {
                current.isGrabbable = true;
                m_dropzone.EnableButton();
            }));
		}
		else
		{
			current.IndexJointUsed = 0;
            current.isDistanceGrabbable = true;

			m_catchedObjectJoint = null;
		}
	}

	protected abstract void OnWorkshopUpdateState(bool state, MTK_Interactable current);
	protected abstract IEnumerator AnimateWorkshop(float duration, VoidDelegate onFinish);
	protected abstract void OnObjectGrabStart();
	protected abstract void OnObjectGrabStay();
}
