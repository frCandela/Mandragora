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

	private void OnEnable()
	{
		m_rgbd = GetComponent<Rigidbody>();
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

			if(m_shell.childCount == 0)
				Destroy(m_shell.gameObject);
		}
	}
}
