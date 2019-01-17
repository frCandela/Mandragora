using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationStar : MonoBehaviour
{
	Constellation m_constellation;
	int m_ID;

	[HideInInspector] public Vector3 m_initPosition;
	Renderer m_renderer;
	Animator m_animator;
	
	[Header("Wwise events")]
	[SerializeField] AK.Wwise.Event m_hit;

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
			{
			    m_hit.Post(gameObject);
				m_animator.SetTrigger("Validated");
			}
			else
				m_animator.SetTrigger("Failed");
		}
	}

	void Start()
	{
		transform.position = m_initPosition;
		m_animator = GetComponent<Animator>();
	}

	public void RegisterConstellation(Constellation c, int ID)
	{
		m_constellation = c;
		m_ID = ID;
	}

	public void TryValidate()
	{
		if(transform.position == m_initPosition) // Init phase
		{
			m_hit.Post(gameObject);
			m_constellation.Init();
		}
		else // Completion phase
		{
			Validated = m_constellation.Check(m_ID);
		}
	}
}
