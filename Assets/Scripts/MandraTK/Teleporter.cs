using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	[SerializeField] MTK_Manager m_mtkManager;

	[Header("Settings")]
	[SerializeField, Range(0,1)]
	float m_tolerance = 0.1f;

	[Header("Fade Time")]
	[SerializeField, Range(0,5)] float m_fadeStart;
	[SerializeField, Range(0,5)] float m_fadeEnd;

	[Header("Sound")]
	[SerializeField] AK.Wwise.Event m_sound;

	TpVfxInstanciate m_tpVFX;
	RaycastHit m_rayHit;
	MTK_TPZone m_targetZone;
	Transform m_targetTransform;
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
					m_targetTransform = m_targetZone.transform;
					m_targetZone.DisplayZone = true;
				}
			}
		}
	}

	bool m_available = true;
	float m_cancelTime;

	bool m_active;
	public bool Active 
	{
		set
		{
			m_active = value;

			if(m_available && !m_active)
			{
				if(TargetZone)
				{
					m_sound.Post(gameObject);
					MTK_Fade.Start(new Color(1,0,1,0), m_fadeStart, MoveMtkManager);
					m_available = false;

					m_tpVFX.LaunchInTpVfx();
				}
			}

			if(!value)
				m_cancelTime = Time.time + m_tolerance;
		}
	}

	private void OnEnable()
	{
		m_tpVFX = GetComponentInChildren<TpVfxInstanciate>();
	}
	
	void Update ()
	{
		if(m_active)
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

		if(!m_active && TargetZone && Time.time > m_cancelTime)
			TargetZone = null;
	}

	private void MoveMtkManager()
	{
		m_mtkManager.transform.position = m_targetTransform.position;
		m_mtkManager.transform.rotation = m_targetTransform.rotation;
		MTK_Fade.Start(Color.clear, m_fadeEnd, () => m_available = true);
	}
}
