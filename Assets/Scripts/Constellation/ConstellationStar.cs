using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationStar : MonoBehaviour
{
	[SerializeField] UnityEvent m_onValidated;
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

			if(Validated)
				m_onValidated.Invoke();
		}
	}

	public void RegisterConstellation(Constellation c)
	{
		m_constellation = c;
	}
}
