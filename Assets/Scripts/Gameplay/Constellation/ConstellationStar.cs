using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationStar : MonoBehaviour
{
	Constellation m_constellation;

	[HideInInspector] public Vector3 m_initPosition;
	Renderer m_renderer;
	Animator m_animator;
	Collider m_collider;

	bool m_validated = false;
	public bool Validated
	{
		get
		{
			return m_validated;
		}
		set
		{
			m_validated = value;

			if(m_validated)
				m_animator.SetTrigger("Validated");
			else
				m_animator.SetTrigger("Failed");

			m_collider.enabled = !m_validated;
		}
	}

	void Start()
	{
		transform.position = m_initPosition;
		m_animator = GetComponent<Animator>();
		m_collider = GetComponent<Collider>();
	}

	public void RegisterConstellation(Constellation c)
	{
		m_constellation = c;
	}

	public void TryValidate()
	{
		if(transform.position == m_initPosition) // Init phase
		{
			m_constellation.Init();
		}
		else // Completion phase
		{
			Validated = m_constellation.Check(this);
		}
	}
}
