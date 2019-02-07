using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
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
	MTK_TPZone[] m_allTPZones;
	Transform m_targetTransform;

	MTK_TPZone m_targetZone;
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
					m_targetZone.Selected = false;

				m_targetZone = value;

				if(m_targetZone)
				{
					m_targetTransform = m_targetZone.transform;
					m_targetZone.Selected = true;
				}
			}
		}
	}

	[SerializeField] MTK_TPZone m_currentZone;
	MTK_TPZone CurrentZone
	{
		get
		{
			return m_currentZone;
		}
		set
		{
			if(value != m_currentZone)
			{
				if(m_currentZone)
					m_currentZone.Selected = false;

				m_currentZone = value;

				if(m_currentZone)
					m_currentZone.Selected = true;
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
			if(enabled)
			{
				AkSoundEngine.PostEvent("Guide_Play", gameObject);

				foreach (MTK_TPZone zone in m_allTPZones)
				{
					if(CurrentZone != zone && zone.m_enabled)
						zone.Active = value;
				}

				m_active = value;

				if(m_available && !m_active)
				{
					if(TargetZone)
					{
						m_sound.Post(gameObject);
						CurrentZone.OnExit();
						MTK_Fade.Start(new Color(1,0,1,0), m_fadeStart, MoveMtkManager);
						m_available = false;

						CurrentZone = TargetZone;
						CurrentZone.Validate();

						m_tpVFX.LaunchInTpVfx();
					}
				}

				if(!value)
				{
					m_cancelTime = Time.time + m_tolerance;
					AkSoundEngine.PostEvent("Guide_Stop", gameObject);
				}
			}
		}
	}

	private void OnEnable()
	{
		m_tpVFX = GetComponentInChildren<TpVfxInstanciate>();
		m_allTPZones = FindObjectsOfType<MTK_TPZone>();
	}
	
	void Update ()
	{
		if(m_active)
		{
			Transform origin =	m_mtkManager.activeSetup.head.transform;
		
			if(Physics.Raycast(origin.position, origin.forward, out m_rayHit, 100, LayerMask.GetMask("TP")))
			{
				MTK_TPZone candidate = m_rayHit.collider.GetComponent<MTK_TPZone>();

				if(candidate && candidate.m_enabled)
					TargetZone = candidate;
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
