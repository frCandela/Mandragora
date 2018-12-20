using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationStar : MonoBehaviour
{
	[SerializeField] UnityEvent m_onValidated;
	[SerializeField] UnityEvent m_onFail;
	Constellation m_constellation;

	[HideInInspector] public Vector3 m_initPosition;
	Renderer m_renderer;
	Animator m_animator;

	public bool m_validated = false;

	void Start()
	{
		transform.position = m_initPosition;
		m_animator = GetComponent<Animator>();
	}

	public void RegisterConstellation(Constellation c)
	{
		m_constellation = c;
	}

	public void TryValidate(bool input)
	{
		if(transform.position == m_initPosition)
		{
			m_constellation.Init();
		}
		else
		{
			if(input)
			{
				m_validated = m_constellation.Check(this);
				m_animator.SetTrigger("Validated");
			}
			else
			{
				m_validated = false;
				m_animator.SetTrigger("Failed");
			}

			if(m_validated)
				m_onValidated.Invoke();
			else
				m_onFail.Invoke();
		}
	}
}
