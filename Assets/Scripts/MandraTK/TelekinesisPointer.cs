using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelekinesisPointer : MonoBehaviour
{
	[SerializeField] ConfigurableJoint m_joint;
	[SerializeField] Outliner m_outliner;
	[SerializeField] FXManager m_fxManager;
	[SerializeField] MTK_InteractHand m_interactHand;
	[SerializeField] Animator m_handAnimator;

	[Header("Settings")]
	[SerializeField, Range(0,10)]
	float m_minDistance = 1;
	[SerializeField, Range(0,10)]
	float m_maxDistance = 5;
	[SerializeField, Range(0,1)]
	float m_minMagnitudeToAttract = .2f;
	[SerializeField, Range(0,1)]
	float m_maxForce = 1;
	[SerializeField, Range(0,1000)]
	float m_forceScale = 300;
	[SerializeField, Range(0,10f)]
	float m_initForceScale = 1;

	[Header("Sound")]
	[SerializeField] AK.Wwise.Event m_wObjectGrabbed;
	[SerializeField] AK.Wwise.Event m_wObjectPlay;
	[SerializeField] AK.Wwise.Event m_wObjectStop; 
	[SerializeField] AK.Wwise.Event m_wHandPlay;
	[SerializeField] AK.Wwise.Event m_wHandStop;

	MTK_InputManager m_inputManager;
	MTK_InteractHand m_hand;
	MTK_InteractiblesManager m_interactiblesManager;

	MTK_Interactable m_currentInteractable;
	RaycastHit m_currentHit;


    public bool isAttracting { get { return m_joint.connectedBody || Target; } private set{} }
    public Rigidbody connectedBody { get { return m_joint.connectedBody; } }
    
    bool m_attract;
	float m_initDistanceToTarget;
	float m_lastForceApplied;
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

					m_inputManager.Haptic(1);
				}
			}			
		}
	}

	void Awake()
	{
		m_inputManager = GetComponentInParent<MTK_InputManager>();
		m_inputManager.m_onTrigger.AddListener(TriggerAttract);

		m_lastPos = transform.position;

		m_hand = GetComponentInParent<MTK_InteractHand>();
		m_hand.m_onTouchInteractable.AddListener(GrabIfTarget);

		m_interactiblesManager = MTK_InteractiblesManager.Instance;
	}

	void Update()
	{
		if(!m_attract)
		{
			if(m_interactHand.Closest)
			{
				Target = null;
			}
			else if(!m_joint.connectedBody)
			{
				Target = m_interactiblesManager.GetClosestToView(transform, 15);
			}
		}
		
		if(Target)
		{
			float distanceScale = Mathf.Min(GetDistanceToTarget() / m_initDistanceToTarget, 1); // 1 -> 0

			// Detect movement to trigger attraction
			if(m_attract)
			{
				Vector3 force = (transform.position - m_lastPos) * 10 * distanceScale;

				if(force.magnitude > m_maxForce)
					force = force.normalized * m_maxForce;
				
				if(force.magnitude > Mathf.Max(m_minMagnitudeToAttract, m_lastForceApplied))
					Attract(force);

				m_inputManager.Haptic(.01f);
			}

			// Apply various forces
			if(m_joint.connectedBody)
			{
				JointDrive drive = m_joint.xDrive;
				drive.positionSpring = 10 + (1 - distanceScale) * m_forceScale * (m_lastForceApplied - m_minMagnitudeToAttract * m_minMagnitudeToAttract);
				drive.positionDamper = 15 * distanceScale + 10;

				m_joint.xDrive = m_joint.yDrive = m_joint.zDrive = drive;

				m_joint.connectedBody.rotation = Quaternion.RotateTowards(m_joint.connectedBody.rotation, transform.rotation, (1 - distanceScale) * 2);
			}
		}

		// Update force
		m_lastPos = transform.position;
	}

    void SetLevitation(MTK_Interactable interactable, bool value)
    {
        Rigidbody rb = interactable.GetComponent<Rigidbody>();
        if(rb )
        {
            if (value)
            {
				interactable.GetComponent<Rigidbody>().isKinematic = false;
                interactable.UseEffects = false;
                rb.velocity = Vector3.up / 20;
                rb.angularVelocity = Random.onUnitSphere;
            }
            rb.useGravity = !value; // !gravity = levitate     
        }         
    }

    bool IsLevitating(MTK_Interactable interactable)
    {
        Rigidbody rb = interactable.GetComponent<Rigidbody>();
        if (rb)
        {
            return !rb.useGravity;
        }
        return false;
    }

    void TriggerAttract(bool input)
	{
		m_lastForceApplied = 0;

		if(input)
		{
			if(Target)
			{
				m_wHandPlay.Post(gameObject);

				m_attract = true;
				Target.UseEffects = false;
                Target.isDistanceGrabbed = true;

                SetLevitation(Target, true);
				m_fxManager.Activate("Grab", Target.transform);
				m_fxManager.Activate("Grab_In", Target.transform);
			}
		}
		else
		{
			m_wHandStop.Post(gameObject);

			m_attract = false;
			m_fxManager.DeActivate("Grab");

			if(Target)
			{
				if(IsLevitating(Target))
					m_fxManager.Activate("Grab_Out", Target.transform);

                SetLevitation(Target, false);

				Target.UseEffects = true;
                Target.isDistanceGrabbed = false;
            }

			if(m_joint.connectedBody)
				UnAttract();
		}

		m_handAnimator.SetBool("Attract", m_attract);
	}

	void Attract(Vector3 force)
	{
        SetLevitation(Target, false);
        m_fxManager.DeActivate("Grab");

		m_joint.connectedBody = Target.GetComponent<Rigidbody>();
		m_joint.connectedBody.AddForce(force.normalized * Mathf.Sqrt(force.magnitude) * m_initForceScale, ForceMode.Impulse);

		m_joint.connectedBody.useGravity = false;
		m_joint.connectedBody.drag = 0;

		m_initDistanceToTarget = GetDistanceToTarget();
		m_lastForceApplied = force.magnitude;

		m_wObjectPlay.Post(Target.gameObject);
	}

	void UnAttract()
	{
		m_wObjectStop.Post(Target.gameObject);

		m_joint.connectedBody.useGravity = true;
		m_joint.connectedBody.drag = 0;

		m_joint.connectedBody = null;

        Target.isDistanceGrabbed = false;

        Target = null;
	}

	void GrabIfTarget(MTK_Interactable input)
	{
		if(m_joint.connectedBody)
		{
			if(m_joint.connectedBody.gameObject == input.gameObject)
			{
				m_inputManager.Haptic(1);

				m_wObjectGrabbed.Post(m_joint.gameObject);
				m_wObjectStop.Post(m_joint.gameObject);
				m_wHandStop.Post(gameObject);

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