using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinder : MTK_Interactable
{
	[SerializeField] GameObject m_planet;
	[SerializeField] Transform m_shell;
	[SerializeField] AK.Wwise.Event m_kinderCreation;
	[SerializeField] float m_minBreakMagnitude;
	[SerializeField, Range(0,1f)] float m_breakThreshold = .4f;
	[SerializeField] ParticleSystem m_hitPs, m_breakPs;

	bool m_enabled;
	Rigidbody m_rgbd;
	int m_piecesCount;

	protected override void OnEnable()
	{
		m_rgbd = GetComponent<Rigidbody>();

		m_piecesCount = (int) (m_shell.childCount * m_breakThreshold);
	}

	void Activate()
	{
		base.OnEnable();
		isDistanceGrabbable = true;
		m_enabled = true;
	}

	public void TriggerKinderSound()
	{
		m_kinderCreation.Post(gameObject);
	}

	public override void Grab(bool input)
	{
		m_rgbd.isKinematic = false;

		base.Grab(input);
	}

	private void OnCollisionEnter(Collision other)
	{
		if(m_enabled)
			m_rgbd.isKinematic = false;

		if(other.relativeVelocity.sqrMagnitude > m_minBreakMagnitude)
		{
			other.contacts[0].thisCollider.gameObject.SetActive(false);
			m_hitPs.transform.position = other.contacts[0].point;
			m_hitPs.transform.LookAt(other.contacts[0].point + other.contacts[0].normal);
			m_hitPs.Emit(Random.Range(1,3));

			m_piecesCount--;

			if(m_piecesCount == 0)
				Break();
		}
	}

	[ContextMenu("Break")]
	void Break()
	{
		Destroy(m_rgbd);
		m_shell.gameObject.SetActive(false);

		m_breakPs.gameObject.SetActive(true);

		// m_planet.transform.SetParent(null, true);
		m_planet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		m_planet.GetComponent<MTK_Interactable>().enabled = true;
	}
}
