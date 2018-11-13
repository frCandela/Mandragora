using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

[RequireComponent(typeof(VRTK_InteractGrab))]
public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField]
	SpringJoint m_joint;
	[SerializeField]
	Transform m_rayPreview;

	[Header("Settings")]
	[SerializeField, Range(0,10)]
	float m_maxDistance = 5;

	VRTK_InteractGrab m_interactGrab;
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

	void Awake()
	{
		m_interactGrab = GetComponent<VRTK_InteractGrab>();

		m_interactGrab.interactTouch.ControllerStartTouchInteractableObject += GrabIfTarget;
		m_interactGrab.GrabButtonReleased += StopAttract;
		m_interactGrab.GrabButtonPressed += StartAttract;
	}

	void Update()
	{
		if(!m_joint.connectedBody)
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

	void StartAttract(object sender, ControllerInteractionEventArgs e)
	{
		if(Target)
		{
			Ungrab();
		}
	}

	void StopAttract(object sender, ControllerInteractionEventArgs e)
	{
		if(m_joint.connectedBody)
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody.drag = 0;
			m_joint.connectedBody = null;

			Target = null;
		}
	}

	void Ungrab()
	{
		m_joint.connectedBody = Target.GetComponent<Rigidbody>();
		m_joint.connectedBody.useGravity = false;
		m_joint.connectedBody.drag = 6;
	}

	void GrabIfTarget(object sender, ObjectInteractEventArgs e)
	{
		if(m_joint.connectedBody)
		{
			if(m_joint.connectedBody.gameObject == e.target)
			{
				m_interactGrab.PerformGrabAttempt(Target.gameObject);
				Ungrab();
			}
		}
	}
}