using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocleSounds : MonoBehaviour
{
	[SerializeField] AK.Wwise.Event m_turnOn;
	[SerializeField] AK.Wwise.Event m_turnOff;
	[SerializeField] AK.Wwise.Event m_ambientOn;
	[SerializeField] AK.Wwise.Event m_ambientOff;

	public bool State
	{
		set
		{
			if(value)
			{
				m_turnOn.Post(gameObject);
				m_ambientOff.Post(gameObject);
			}
			else
			{
				m_turnOff.Post(gameObject);
				m_ambientOn.Post(gameObject);
			}
		}
	}
}
