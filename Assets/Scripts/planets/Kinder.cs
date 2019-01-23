using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinder : MTK_Interactable
{
	[SerializeField] GameObject m_planet;
	[SerializeField] Transform m_shell;
	[SerializeField] AK.Wwise.Event m_kinderCreation;
	[SerializeField] float m_minBreakMagnitude;

	bool m_enabled;
	Rigidbody m_rgbd;
	ParticleSystem m_ps;

	private void OnEnable()
	{
		m_rgbd = GetComponent<Rigidbody>();
		m_ps = GetComponentInChildren<ParticleSystem>();
	}

	void Activate()
	{
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
			m_ps.transform.position = other.contacts[0].point;
			m_ps.transform.LookAt(other.contacts[0].point + other.contacts[0].normal);
			m_ps.Emit(Random.Range(1,3));

			if(m_shell.childCount == 0)
				Destroy(m_shell.gameObject);
		}
	}
}
