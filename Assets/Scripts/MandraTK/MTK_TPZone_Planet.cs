using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MTK_TPZone_Planet : MTK_TPZone
{
	[SerializeField] SolarSystem m_solarSystem;

	IcoPlanet m_planet;
	public IcoPlanet Planet
	{
		get
		{
			return m_planet;
		}
		set
		{
			if(m_planet)
			{
				ParticleSystem.EmissionModule emission;
				emission = m_planet.transform.GetChild(0).GetComponentInChildren<ParticleSystem>().emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			}

			m_planet = value;

			gameObject.SetActive(m_planet);
		}
	}

	public override bool Active
	{
		set
		{
		}
	}

	public override bool Selected
	{
		set
		{
			if(value)
				AkSoundEngine.PostEvent("Play_Look_TP", gameObject);

			ParticleSystem.EmissionModule emission;
			emission = m_planet.transform.GetChild(0).GetComponentInChildren<ParticleSystem>().emission;

			if(value)
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(30);
			else
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
		}
	}

	public override void Validate()
	{
	}

	public override void Appears(bool input)
	{
	}

	public override void OnExit()
	{
	}
}
