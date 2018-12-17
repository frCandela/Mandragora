using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Constellation : MonoBehaviour
{
	[SerializeField] float m_timeToMove;
	[SerializeField] UnityEvent m_onCompleted;
	
	ConstellationStar[] m_stars;
	Transform[] m_starsTransform;
	Vector3[] m_starsInitPosition;

	int m_nextStarID = 0;

	// Use this for initialization
	void Start ()
	{
		m_stars = GetComponentsInChildren<ConstellationStar>();
		m_starsTransform = new Transform[m_stars.Length];
		m_starsInitPosition = new Vector3[m_stars.Length];

		for (int i = 0; i < m_stars.Length; i++)
		{
			m_stars[i].RegisterConstellation(this);
			m_starsTransform[i] = m_stars[i].transform;
			m_starsInitPosition[i] = m_starsTransform[i].localPosition;
		}
	}
	
	public bool Check(ConstellationStar input)
	{
		if (input == m_stars[m_nextStarID])
		{
			m_nextStarID++;
			if(m_nextStarID == m_stars.Length)
				Complete();

			return true;
		}

		// if fail
		foreach (var star in m_stars)
			star.TryValidate(false);

		m_nextStarID = 0;

		return false;
	}

	[ContextMenu("Complete")]
	void Complete()
	{
		foreach (var star in m_stars)
			Destroy(star);

		StartCoroutine(goToCenter());
	}

	IEnumerator goToCenter()
	{
		for (float t = 0; t < 1; t += Time.fixedDeltaTime / m_timeToMove)
		{
			for (int i = 0; i < m_starsTransform.Length; i++)
				m_starsTransform[i].localPosition = Vector3.Lerp(m_starsInitPosition[i], Vector3.zero, t);

			yield return new WaitForEndOfFrame();
		}

		foreach (var tr in m_starsTransform)
			Destroy(tr.gameObject);

		m_onCompleted.Invoke();
	}
}
