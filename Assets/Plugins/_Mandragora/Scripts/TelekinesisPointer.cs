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
	[SerializeField]
	Transform m_rayPreview;

	[Header("Settings")]
	[SerializeField, Range(0,10)]
	float m_maxDistance = 5;

	VRTK_InteractableObject m_currentInteractable;
	RaycastHit m_currentHit;
	

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
			Ray newRay = new Ray(transform.position, transform.forward);
			// Update Target
			if (Physics.Raycast(newRay, out m_currentHit, m_maxDistance))
			{
				Target = m_currentHit.collider.GetComponent<VRTK_InteractableObject>();

				m_rayPreview.localScale = new Vector3(0.003f, 0.003f, m_currentHit.distance);
				m_rayPreview.localPosition = new Vector3(0, 0, m_currentHit.distance / 2);
			}
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
		enabled = false;

		if(m_joint.connectedBody)
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody.drag = 0;
			m_joint.connectedBody = null;

			Target = null;
		}
	}
}