using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_TPZone : MonoBehaviour
{
	Animator m_animator;
	[SerializeField] UnityEvent m_onExitZone;
	[SerializeField] Animator m_pipeAnimator;

	public bool m_enabled = false;

	public virtual bool Active
	{
		set
		{
			if(m_animator)
				m_animator.SetBool("Active", value);
		}
	}

	public virtual bool Selected
	{
		set
		{
			if(m_animator)
				m_animator.SetBool("Selected", value);

			if(value)
				AkSoundEngine.PostEvent("Play_Look_TP", gameObject);
		}
	}

	private void Awake()
	{
		m_animator = GetComponent<Animator>();
	}

	public virtual void Validate()
	{
		if(m_animator)
			m_animator.SetTrigger("Validate");
	}

	public virtual void Appears(bool input)
	{
		if(!input)
			if(!m_enabled)
			{
				AkSoundEngine.PostEvent("Play_Way", gameObject);

				if(m_animator)
					m_animator.SetTrigger("Appears");

				if(m_pipeAnimator)
					m_pipeAnimator.SetTrigger("Trigger");
					
				m_enabled = true;
			}
	}

	public virtual void OnExit()
	{
		Constellation constellation = transform.parent.GetComponentInChildren<Constellation>();
		if(constellation)
			constellation.Recycle();

		m_onExitZone.Invoke();
	}
}
