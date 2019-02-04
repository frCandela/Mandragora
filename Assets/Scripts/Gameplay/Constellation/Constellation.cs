using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Constellation : MonoBehaviour
{
	[SerializeField] AnimationCurve m_moveLerpCurve_Init;
	[SerializeField] AnimationCurve m_moveLerpCurve_Complete;
	[SerializeField] AnimationCurve m_noiseAmountCurve;
	[SerializeField] float m_trailSpeed = 1;
	[SerializeField] float m_maxDistance = 1;
	[SerializeField] float m_formationDistance = 1;
	[SerializeField] float m_recycleTime = 10;
	[SerializeField] GameObject m_kinder;
	[SerializeField] Transform m_trailFX;
	[SerializeField] GameObject m_next;
	[SerializeField] UnityEvent m_onComplete;

	[Header("Wwise events")]
	[SerializeField] AK.Wwise.Event m_validated;
	
	ConstellationStar[] m_stars;
	Transform[] m_starsTransform;
	Vector3[] m_starsInitPosition;
	LineRenderer m_lineRenderer;
	TrailRenderer m_trail;
	[HideInInspector] public Transform m_transformFollow;

	int m_currentStarID = -1;
	int m_endStarID = -1;
	int m_direction = -1;
	int m_trailDestination = 0;

	// Use this for initialization
	void Awake ()
	{
		m_trail = GetComponentInChildren<TrailRenderer>();

		m_stars = GetComponentsInChildren<ConstellationStar>();
		m_starsTransform = new Transform[m_stars.Length];
		m_starsInitPosition = new Vector3[m_stars.Length];

		m_lineRenderer = GetComponent<LineRenderer>();
		m_lineRenderer.positionCount = m_stars.Length;
		m_lineRenderer.enabled = false;

		for (int i = 0; i < m_stars.Length; i++)
		{
			m_stars[i].RegisterConstellation(this, i);
			m_starsTransform[i] = m_stars[i].transform;
			m_starsInitPosition[i] = m_starsTransform[i].localPosition;
		}
	}

	private void Update()
	{
		Vector3 trailDestination;

		if(m_trail.emitting)
		{
			int currentDestination = (int)Mathf.PingPong(m_trailDestination, m_starsTransform.Length - 1);
			trailDestination = m_starsTransform[currentDestination].localPosition + m_starsTransform[currentDestination].GetChild(0).localPosition;
			m_trail.transform.localPosition = Vector3.MoveTowards(m_trail.transform.localPosition, trailDestination, m_trailSpeed * Time.deltaTime);

			if(Vector3.Distance(m_trail.transform.localPosition, trailDestination) < 0.1f)
				m_trailDestination++;
		}

		Vector3 handPos;

		if(m_transformFollow && m_currentStarID != m_endStarID && m_currentStarID != -1)
		{
			handPos = transform.InverseTransformPoint(m_transformFollow.position);

			m_lineRenderer.SetPosition(m_endStarID, handPos);

			m_trailFX.localPosition = handPos;
			
			if(Vector3.Distance(m_transformFollow.position, transform.TransformPoint(m_lineRenderer.GetPosition(m_currentStarID))) > m_maxDistance)
				Fail();
		}
	}
	
	public bool Check(int m_ID, Transform tr)
	{
		m_trail.emitting = false;
		m_transformFollow = tr;

		if(m_currentStarID == -1) // set first star
		{
			// First of last Only
			if(m_ID == 0 || m_ID == m_starsTransform.Length-1)
			{
				m_currentStarID = m_ID;
				m_direction = m_ID == 0 ? 1 : -1;
				m_endStarID = m_ID == 0 ? m_starsTransform.Length-1 : 0;

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
					for (int i = 0; i < m_starsTransform.Length; i++)
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
		for (int i = m_currentStarID; i != m_endStarID + m_direction; i += m_direction)
			m_lineRenderer.SetPosition(i, m_starsTransform[m_currentStarID].localPosition + m_starsTransform[m_currentStarID].GetChild(0).localPosition);
	}

	void Fail()
	{
		for (int i = 0; i < m_starsTransform.Length; i++)
		{
			m_stars[i].Validated = false;
			m_lineRenderer.SetPosition(i, m_starsInitPosition[0]);
		}

		m_currentStarID = -1;
		m_trail.emitting = true;
	}

	[ContextMenu("Complete")]
	void Complete()
	{
		m_validated.Post(gameObject);
		m_kinder.SetActive(true);
		m_trail.enabled = false;
		m_onComplete.Invoke();

		Vector3[] destination = new Vector3[m_starsTransform.Length];
		for (int i = 0; i < destination.Length; i++)
			destination[i] = m_kinder.transform.localPosition;
		
		StartCoroutine(
			MoveTo(destination, 1.5f, 5, m_moveLerpCurve_Complete, () =>
			{
				foreach (var tr in m_starsTransform)
					Destroy(tr.gameObject, 1);
				
				m_kinder.transform.SetParent(transform.parent, true);

				Destroy(m_lineRenderer);
				Invoke("Recycle", m_recycleTime);
			}));
	}

	void Recycle()
	{
        if(m_next)
		    Instantiate(m_next, transform.parent.position, transform.rotation, transform.parent);

		Destroy(gameObject);
	}

	public void Init(Transform tr)
	{
		StartCoroutine(Place(1.5f, tr));
		
		Vector3 initPos = m_starsTransform[0].localPosition + m_starsTransform[0].GetChild(0).localPosition;
		m_trail.transform.localPosition = initPos;

		StartCoroutine(MoveTo(m_starsInitPosition, 3, 20, m_moveLerpCurve_Init, () =>
		{
			m_lineRenderer.enabled = true;

			for (int i = 0; i < m_starsTransform.Length; i++)
				m_lineRenderer.SetPosition(i, initPos);

			m_trailDestination = 0;
			m_trail.emitting = true;
		}));
	}

	IEnumerator MoveTo(Vector3[] destinations, float timeToMove, float noiseScale, AnimationCurve curve, VoidDelegate endAction = null)
	{
		float lenght = m_starsTransform.Length/10f;

		Vector3[] startPositions = new Vector3[m_starsTransform.Length];

		for (int i = 0; i < m_starsTransform.Length; i++)
			startPositions[i] = m_starsTransform[i].localPosition;

		for (float t = 0; t < 1; t += Time.fixedDeltaTime / timeToMove)
		{
			for (int i = 0; i < m_starsTransform.Length; i++)
			{
				m_starsTransform[i].localPosition = Vector3.Lerp(startPositions[i], destinations[i] + transform.rotation * transform.localPosition, curve.Evaluate(t))
				+ (new Vector3(Mathf.PerlinNoise(t, i), Mathf.PerlinNoise(t, i + 1), Mathf.PerlinNoise(t, i + 2)) - Vector3.one/2)
					* noiseScale * m_noiseAmountCurve.Evaluate(t);

				m_lineRenderer.SetPosition(i, m_starsTransform[i].localPosition);
			}

			yield return new WaitForEndOfFrame();
		}

		if(endAction != null)
			endAction.Invoke();
	}

	IEnumerator Place(float timeToMove, Transform hand)
	{
		Vector3 playerPos = Camera.main.transform.position;
		playerPos.Scale(new Vector3(1,0,1));

		Vector3 thisPos = hand.position;
		thisPos.Scale(new Vector3(1,0,1));

		Vector3 targetPos = transform.position + (thisPos - playerPos).normalized * m_formationDistance;

		transform.LookAt(targetPos);

		for (float t = 0; t < 1; t += Time.fixedDeltaTime / timeToMove)
		{
			transform.position = Vector3.Lerp(transform.position, targetPos, t);

			m_trailFX.position = transform.position;

			yield return new WaitForEndOfFrame();
		}
	}
}
