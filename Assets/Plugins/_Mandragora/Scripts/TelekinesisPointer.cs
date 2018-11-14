using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField]
	ConfigurableJoint m_joint;
	[SerializeField]
	Transform m_rayPreview;

	[Header("Settings")]
	[SerializeField, Range(0,10)]
	float m_maxDistance = 5;
	[SerializeField, Range(0,50)]
	float m_minMangitudeToAttract = 5;
	[SerializeField, Range(0,1000)]
	float m_forceScale = 500;
	[SerializeField, Range(0,10)]
	float m_initForceScale = 2;

	VRTK_InteractGrab m_interactGrab;

	VRTK_InteractableObject m_currentInteractable;
	RaycastHit m_currentHit;
	bool m_attract;
	float m_initDistanceToTarget;

	List<Vector3> m_velList = new List<Vector3>(5);
	Vector3 m_lastPos;

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
		m_interactGrab = GetComponentInParent<VRTK_InteractGrab>();

		m_interactGrab.interactTouch.ControllerStartTouchInteractableObject += GrabIfTarget;
		m_interactGrab.GrabButtonReleased += StopAttract;
		m_interactGrab.GrabButtonPressed += StartAttract;

		m_lastPos = transform.position;
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

		// Update Velocity
		m_velList.Add(transform.position - m_lastPos);

		if (m_velList.Count > 4)
			m_velList.RemoveAt(0);
			
		m_lastPos = transform.position;

		if(m_attract && Target)
		{
			Vector3 force = GetVelocity() * 1000;
			
			if(force.magnitude > m_minMangitudeToAttract)
				Attract(force);
		}

		if(m_joint.connectedBody)
		{
			JointDrive drive = m_joint.xDrive;
			drive.positionSpring = 50 + (1 - GetDistanceToTarget() / m_initDistanceToTarget) * m_forceScale;

			m_joint.xDrive = m_joint.yDrive = m_joint.zDrive = drive;
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
			Attract(Vector3.zero);
	}

	void Attract(Vector3 force)
	{
		if(force != Vector3.zero)
		{
			m_attract = false;

			m_joint.connectedBody = Target.GetComponent<Rigidbody>();
			m_joint.connectedBody.AddForce(force * m_initForceScale, ForceMode.Impulse);
			m_joint.connectedBody.angularVelocity = force;

			m_joint.connectedBody.useGravity = false;
			m_joint.connectedBody.drag = 0;

			m_initDistanceToTarget = GetDistanceToTarget();
		}
		else
		{
			m_joint.connectedBody.useGravity = true;
			m_joint.connectedBody.drag = 0;

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
				VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(m_interactGrab.gameObject), 1);

				m_interactGrab.PerformGrabAttempt(Target.gameObject);
				Attract(Vector3.zero);
			}
		}
	}

	float GetDistanceToTarget()
	{
		return Vector3.Distance(transform.position, Target.transform.position);
	}

	Vector3 GetVelocity()
	{
		Vector3 angularVelocity = Vector3.zero;
		foreach (Vector3 vel in m_velList)
			angularVelocity += vel;

		return angularVelocity / m_velList.Count;
	}
}