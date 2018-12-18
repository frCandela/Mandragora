using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_TPZone : MonoBehaviour
{
	Animator m_animator;

	public bool DisplayZone
	{
		set
		{
			m_animator.SetBool("Selected", value);
		}
	}

	void Start ()
	{
		m_animator = GetComponent<Animator>();
	}
}
