using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField]
	ConfigurableJoint m_joint;
	[SerializeField]
	Transform m_rayPreview;
	[SerializeField]
	Outliner m_outliner;

	[Header("Settings")]
	[SerializeField, Range(0,10)]
	float m_maxDistance = 5;
	[SerializeField, Range(0,1)]
	float m_minMagnitudeToAttract = .2f;
	[SerializeField, Range(0,1000)]
	float m_forceScale = 300;
	[SerializeField, Range(0,10f)]
	float m_initForceScale = 1;
	[SerializeField] AK.Wwise.Event m_wOnAttract;
	[SerializeField] AK.Wwise.Event m_wOnGrab;

	MTK_InputManager m_inputManager;
	MTK_InteractHand m_hand;

	MTK_Interactable m_currentInteractable;
	RaycastHit m_currentHit;

    public bool isAttracting { get { return m_joint.connectedBody; } private set{} }
	bool m_attract;
	float m_initDistanceToTarget;
	Vector3 m_lastForceApplied;
	Vector3 m_lastPos;

	MTK_Interactable Target
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
					m_outliner.OultineOff(m_currentInteractable);
					m_currentInteractable = value;
				}
			}
			else
			{
				if(m_currentInteractable != value)
				{
					if(m_currentInteractable)
						m_outliner.OultineOff(m_currentInteractable);

					m_currentInteractable = value;
					m_outliner.OultineOn(m_currentInteractable);
				}
			}			
		}
	}

	void Awake()
	{
		m_inputManager = GetComponentInParent<MTK_InputManager>();
		m_inputManager.m_onGrip.AddListener(TriggerAttract);

		m_lastPos = transform.position;

		m_hand = GetComponentInParent<MTK_InteractHand>();
		m_hand.m_onTouchInteractable.AddListener(GrabIfTarget);
	}

	void Update()
	{
		if(!m_attract && !m_joint.connectedBody)
		{
			Ray newRay = new Ray(transform.position, transform.forward);
			// Update Target
			if (Physics.Raycast(newRay, out m_currentHit, m_maxDistance))
			{
				Target = m_currentHit.transform.GetComponent<MTK_Interactable>();

				if(!Target)
					Target = m_currentHit.transform.GetComponentInParent<MTK_Interactable>();

				m_rayPreview.localScale = new Vector3(0.01f, 0.01f, m_currentHit.distance);
				m_rayPreview.localPosition = new Vector3(0, 0, m_currentHit.distance / 2);
			}
			else
			{
				Target = null;
				m_rayPreview.localScale = new Vector3(0.01f, 0.01f, m_maxDistance);
				m_rayPreview.localPosition = new Vector3(0, 0, m_maxDistance / 2);
			}
		}

		if(Target)
		{
			float distanceScale = Mathf.Min(GetDistanceToTarget() / m_initDistanceToTarget, 1); // 1 -> 0

			// Detect movement to trigger attraction
			if(m_attract)
			{
				Vector3 force = (transform.position - m_lastPos) * 20 * distanceScale; // Scale from 0 to 1

				if(force.sqrMagnitude > 1)
					force.Normalize();
				
				if(force.magnitude > m_minMagnitudeToAttract && force.sqrMagnitude > m_lastForceApplied.sqrMagnitude)
					Attract(force);
			}

			// Apply various forces
			if(m_joint.connectedBody)
			{
				JointDrive drive = m_joint.xDrive;
				drive.positionSpring = 10 + (1 - distanceScale) * m_forceScale * (m_lastForceApplied.sqrMagnitude - m_minMagnitudeToAttract * m_minMagnitudeToAttract);
				drive.positionDamper = 15 * distanceScale + 5;

				m_joint.xDrive = m_joint.yDrive = m_joint.zDrive = drive;

				m_joint.connectedBody.rotation = Quaternion.RotateTowards(m_joint.connectedBody.rotation, transform.rotation, (1 - distanceScale) * 2);
			}
		}

		// Update force
		m_lastPos = transform.position;
	}

	void TriggerAttract(bool input)
	{
		if(input)
		{
			m_attract = true;
		}
		else
		{
			m_attract = false;
			if(m_joint.connectedBody)
				UnAttract();
		}
	}

	void Attract(Vector3 force)
	{
		m_wOnAttract.Post(Target.gameObject);

		m_joint.connectedBody = Target.GetComponent<Rigidbody>();
		m_joint.connectedBody.AddForce(force.normalized * Mathf.Sqrt(force.magnitude) * m_initForceScale, ForceMode.Impulse);

		m_joint.connectedBody.useGravity = false;
		m_joint.connectedBody.drag = 0;

		m_initDistanceToTarget = GetDistanceToTarget();
		m_lastForceApplied = force;
	}

	void UnAttract()
	{
		m_joint.connectedBody.useGravity = true;
		m_joint.connectedBody.drag = 0;

		m_joint.connectedBody = null;
		m_lastForceApplied = Vector3.zero;

		Target = null;
	}

	void GrabIfTarget(MTK_Interactable input)
	{
		if(m_joint.connectedBody)
		{
			if(m_joint.connectedBody.gameObject == input.gameObject)
			{
				m_wOnGrab.Post(Target.gameObject);
				
				m_inputManager.Haptic(1);

				input.transform.rotation = transform.rotation;
				input.transform.position = transform.position;
				m_hand.Grab(Target);
				UnAttract();
			}
		}
	}

	float GetDistanceToTarget()
	{
		return Vector3.Distance(transform.position, Target.transform.position);
	}
}