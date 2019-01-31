using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_TPZone : MonoBehaviour
{
	Animator m_animator;

	bool m_apparu = false;

	public bool Active
	{
		set
		{
			m_animator.SetBool("Active", value);
		}
	}

	public bool Selected
	{
		set
		{
			m_animator.SetBool("Selected", value);
		}
	}

	private void Awake()
	{
		m_animator = GetComponent<Animator>();
	}

	public void Validate()
	{
		m_animator.SetTrigger("Validate");
	}

	public void Appears(bool input)
	{
		if(!input)
			if(!m_apparu)
			{
				m_animator.SetTrigger("Appears");
				m_apparu = true;
			}
	}
}
