using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Constellation : MonoBehaviour
{
	[SerializeField] float m_timeToMove;
	[SerializeField] float m_animationAmplitude = 5;
	[SerializeField] UnityEvent m_onCompleted;

	[Header("Wwise events")]
	[SerializeField] AK.Wwise.Event m_validated;
	
	ConstellationStar[] m_stars;
	Transform[] m_starsTransform;
	Vector3[] m_starsInitPosition;
	LineRenderer m_lineRenderer;

	int m_currentStarID = -1;
	int m_endStarID = -1;
	int m_direction = -1;

	// Use this for initialization
	void Awake ()
	{
		m_lineRenderer = GetComponent<LineRenderer>();
		m_stars = GetComponentsInChildren<ConstellationStar>();
		m_starsTransform = new Transform[m_stars.Length];
		m_starsInitPosition = new Vector3[m_stars.Length];

		for (int i = 0; i < m_stars.Length; i++)
		{
			m_stars[i].RegisterConstellation(this, i);
			m_starsTransform[i] = m_stars[i].transform;
			m_starsInitPosition[i] = m_starsTransform[i].localPosition;
		}
	}
	
	public bool Check(int m_ID)
	{
		if(m_currentStarID == -1) // set first star
		{
			// First of last Only
			if(m_ID == 0 || m_ID == m_stars.Length-1)
			{
				m_currentStarID = m_ID;
				m_direction = m_ID == 0 ? 1 : -1;
				m_endStarID = m_ID == 0 ? m_stars.Length-1 : 0;

				UpdateLine();
				
				return true;
			}
			
			return false;
		}
		else // check if neighbor
		{
			if(Mathf.Abs(m_currentStarID - m_ID) == 1)
			{
				m_currentStarID = m_ID;
				UpdateLine();

				if(m_currentStarID == m_endStarID) // Check constellation is complete
				{
					for (int i = 0; i < m_stars.Length; i++)
					{
						if(m_currentStarID != i && m_stars[i].Validated == false)
						{
							Fail();
							return false;
						}
					}

					Complete();
				}

				return true;
			}
		}

		Fail();
		return false;
	}

	void UpdateLine()
	{
		for (int i = m_currentStarID; i != m_endStarID +1; i += m_direction)
			m_lineRenderer.SetPosition(i, m_starsInitPosition[m_currentStarID]);
	}

	void Fail()
	{
		for (int i = 0; i < m_stars.Length; i++)
		{
			m_stars[i].Validated = false;
			m_lineRenderer.loop = true;
			m_lineRenderer.SetPosition(i, m_starsInitPosition[0]);
		}

		m_currentStarID = -1;
	}

	[ContextMenu("Complete")]
	void Complete()
	{
		m_validated.Post(gameObject);
		StartCoroutine(goFromTo(0, 1, m_onCompleted.Invoke));
	}

	public void Init()
	{
		m_lineRenderer.positionCount = m_stars.Length;

		for (int i = 0; i < m_stars.Length; i++)
		{
			m_starsTransform[i].localPosition = m_starsInitPosition[i];
			m_lineRenderer.SetPosition(i, m_starsInitPosition[0]);
		}
	}

	IEnumerator goFromTo(float start, float end, VoidDelegate endAction = null)
	{
		float lenght = m_starsTransform.Length/10f;

		if(endAction != null)
			endAction.Invoke();

		for (float t = start; t < end; t += Time.fixedDeltaTime / m_timeToMove)
		{
			for (int i = 0; i < m_starsTransform.Length; i++)
			{
				m_starsTransform[i].localPosition = Vector3.Lerp(m_starsInitPosition[i], Vector3.zero, t)
				+ (new Vector3(Mathf.PerlinNoise(t, i/lenght), Mathf.PerlinNoise(t, i/lenght + 1f/3), Mathf.PerlinNoise(t, i/lenght + 2f/3)) - Vector3.one/2) * m_animationAmplitude * Mathf.Sin(t * Mathf.PI);

				m_lineRenderer.SetPosition(i, m_starsTransform[i].localPosition);
			}

			yield return new WaitForEndOfFrame();
		}

		foreach (var tr in m_starsTransform)
			Destroy(tr.gameObject);
	}
}
