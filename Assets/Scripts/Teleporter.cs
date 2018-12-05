using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	[SerializeField] MTK_Manager m_mtkManager;
	RaycastHit m_rayHit;

	[Header("Fade Time")]
	[SerializeField, Range(0,1)] float m_fadeStart;
	[SerializeField, Range(0,1)] float m_fadeEnd;

	MTK_TPZone m_targetZone;
	Vector3 m_targetPos;
	MTK_TPZone TargetZone
	{
		get
		{
			return m_targetZone;
		}
		set
		{
			if(value != m_targetZone)
			{
				if(m_targetZone)
					m_targetZone.DisplayZone = false;

				m_targetZone = value;

				if(m_targetZone)
				{
					m_targetPos = m_targetZone.GetDestinationPos();
					m_targetZone.DisplayZone = true;
				}
			}
		}
	}

	bool m_available = true;
	
	void Update ()
	{
		Transform origin =	m_mtkManager.activeSetup.head.transform;
		
		if(Physics.Raycast(origin.position, origin.forward, out m_rayHit, 100, LayerMask.GetMask("TP")))
		{
			TargetZone = m_rayHit.collider.GetComponent<MTK_TPZone>();
		}
		else
		{
			TargetZone = null;
		}
	}

	public void Teleport(bool inputValue)
	{
		if(m_available && inputValue)
		{
			if(TargetZone)
			{
				MTK_Fade.Start(Color.black, m_fadeStart, MoveMtkManager);
				m_available = false;
			}
		}
	}

	private void MoveMtkManager()
	{
		m_mtkManager.transform.position = m_targetPos;
		MTK_Fade.Start(Color.clear, m_fadeEnd, () => m_available = true);
	}
}
