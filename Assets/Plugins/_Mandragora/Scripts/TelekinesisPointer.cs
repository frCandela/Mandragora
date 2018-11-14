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
	[SerializeField, Range(0,50)]
	float m_minMangitudeToAttract = 5;

	VRTK_InteractGrab m_interactGrab;
	AngularVelocityTracker m_angularVelocity;

	VRTK_InteractableObject m_currentInteractable;
	RaycastHit m_currentHit;
	bool m_attract;

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
		m_angularVelocity = GetComponent<AngularVelocityTracker>();

		m_interactGrab.interactTouch.ControllerStartTouchInteractableObject += GrabIfTarget;
		m_interactGrab.GrabButtonReleased += StopAttract;
		m_interactGrab.GrabButtonPressed += StartAttract;
	}

	void Update()
	{
		if(!m_attract && !m_joint.connectedBody && !m_interactGrab.CanRelease())
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
		else
		{
			m_rayPreview.localScale = Vector3.zero;
			m_rayPreview.localPosition = Vector3.zero;
		}

		if(m_attract && Target)
		{
			float force = m_angularVelocity.GetAngularVelocity().magnitude;
			if(force > m_minMangitudeToAttract)
				Attract(force*10);
		}
	}

	void StartAttract(object sender, ControllerInteractionEventArgs e)
	{
		m_attract = true;
	}

	void StopAttract(object sender, ControllerInteractionEventArgs e)
	{
		m_attract = false;
		if(m_joint.connectedBody)
			Attract(0);
	}

	void Attract(float force)
	{
		if(force > 0)
		{
			m_joint.connectedBody = Target.GetComponent<Rigidbody>();

			m_joint.connectedBody.useGravity = false;
			m_joint.connectedBody.drag = 6;
			m_joint.spring = force;
		}
		else
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody.drag = 0;
			m_joint.spring = 0;

			m_joint.connectedBody = null;

			Target = null;
		}
	}

	void GrabIfTarget(object sender, ObjectInteractEventArgs e)
	{
		if(m_joint.connectedBody)
		{
			if(m_joint.connectedBody.gameObject == e.target)
			{
				VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), 1);

				m_interactGrab.PerformGrabAttempt(Target.gameObject);
				Attract(0);
			}
		}
	}
}