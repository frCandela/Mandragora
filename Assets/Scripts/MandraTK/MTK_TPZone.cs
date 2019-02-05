using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_TPZone : MonoBehaviour
{
	Animator m_animator;
	[SerializeField] UnityEvent m_onExitZone;

	public bool m_enabled = false;

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

			if(value)
				AkSoundEngine.PostEvent("Play_Look_TP", gameObject);
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
			if(!m_enabled)
			{
				AkSoundEngine.PostEvent("Play_Way", gameObject);
				m_animator.SetTrigger("Appears");
				m_enabled = true;
			}
	}

	public void OnExit()
	{
		m_onExitZone.Invoke();
	}
}
