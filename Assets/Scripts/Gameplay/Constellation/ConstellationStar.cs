using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationStar : MonoBehaviour
{
	[SerializeField] UnityEvent m_onValidated;
	[SerializeField] UnityEvent m_onFail;
	Constellation m_constellation;

	Renderer m_renderer;

	public bool m_validated = false;

	public void RegisterConstellation(Constellation c)
	{
		m_constellation = c;
	}

	public void TryValidate(bool input)
	{
		if(input)
			m_validated = m_constellation.Check(this);
		else
			m_validated = false;

		if(m_validated)
			m_onValidated.Invoke();
		else
			m_onFail.Invoke();
	}
}
