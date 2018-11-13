using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField]
	VRTK_ControllerEvents m_controllerEvents;
	[SerializeField]
	SpringJoint m_joint;

	RaycastHit m_currentHit;
	VRTK_InteractableObject m_currentInteractable;
	bool m_isAttracting;

	public delegate void OnEndAttract(GameObject go);
	public OnEndAttract m_onEndAttract;

	VRTK_InteractableObject Target
	{
		set
		{
			if(value == null)
			{
				if(m_currentInteractable)
				{
					m_currentInteractable.ToggleHighlight(false);
					m_currentInteractable = value;
				}
			}
			else
			{
				if(m_currentInteractable != value)
				{
					if(m_currentInteractable)
						m_currentInteractable.ToggleHighlight(false);

					m_currentInteractable = value;
					m_currentInteractable.ToggleHighlight(true);
				}
			}			
		}
	}

	void Update()
	{
		if(m_isAttracting)
		{
			if(Vector3.Distance(m_joint.connectedBody.position, transform.position) < 1)
			{
				m_onEndAttract.Invoke(m_joint.connectedBody.gameObject);
				StopAttract();
			}
		}
	}

	public void StartAttract()
	{
		// Update Target
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out m_currentHit, Mathf.Infinity))
			Target = m_currentHit.collider.GetComponent<VRTK_InteractableObject>();

		if(m_currentInteractable)
		{
			m_isAttracting = true;

			m_joint.connectedBody = m_currentInteractable.GetComponent<Rigidbody>();
			m_joint.connectedBody.useGravity = false;
		}
	}

	public void StopAttract()
	{
		m_isAttracting = false;

		if(m_joint.connectedBody)
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody = null;
		}
	}
}