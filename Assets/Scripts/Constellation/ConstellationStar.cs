using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationStar : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] Color m_colorIdle;
	[SerializeField] Color m_colorOk;

	Constellation m_constellation;

	Renderer m_renderer;

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
			m_constellation.Check();
			
			m_renderer.material.color = m_validated ? m_colorOk : m_colorIdle;
		}
	}

	void Start()
	{
		m_renderer = GetComponent<Renderer>();
	}

	public void RegisterConstellation(Constellation c)
	{
		m_constellation = c;
	}
}
