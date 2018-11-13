using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField]
	VRTK_InteractGrab m_interactGrab;
	[SerializeField]
	SpringJoint m_joint;

	RaycastHit m_currentHit;
	VRTK_InteractableObject m_currentInteractable;

	VRTK_InteractableObject Target
	{
		get
		{
			return m_currentInteractable;
		}
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
		if(m_joint.connectedBody)
		{
			if(m_joint.connectedBody.gameObject == m_interactGrab.GetGrabbableObject())
			{
				m_interactGrab.PerformGrabAttempt(Target.gameObject);
				StopAttract();
			}
		}
		else
		{
			// Update Target
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out m_currentHit, Mathf.Infinity))
				Target = m_currentHit.collider.GetComponent<VRTK_InteractableObject>();
		}
	}

	public void StartAttract()
	{
		if(Target)
		{
			m_joint.connectedBody = Target.GetComponent<Rigidbody>();
			m_joint.connectedBody.useGravity = false;
			m_joint.connectedBody.drag = 4;
		}
	}

	public void StopAttract()
	{
		if(m_joint.connectedBody)
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody.drag = 0;
			m_joint.connectedBody = null;
			Target = null;
		}
	}
}