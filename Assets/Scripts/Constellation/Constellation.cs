using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
	ConstellationStar[] m_stars;

	// Use this for initialization
	void Start ()
	{
		m_stars = GetComponentsInChildren<ConstellationStar>();

		foreach (var star in m_stars)
			star.RegisterConstellation(this);
	}
	
	public void Check()
	{
		foreach (var star in m_stars)
		{
			if(!star.Validated)
				return;
		}

		Destroy(gameObject);
	}
}
