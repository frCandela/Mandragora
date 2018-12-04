using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	[SerializeField] MTK_Manager m_mtkManager;
	RaycastHit m_rayHit;

	[Header("Fade Time")]
	[SerializeField, Range(0,1)] float m_fadeStart;
	[SerializeField, Range(0,1)] float m_fadeEnd;

	Vector3 m_targetPosition;
	
	void Update ()
	{
		Transform origin =	m_mtkManager.activeSetup.head.transform;

		if(Physics.Raycast(origin.position, origin.forward, out m_rayHit, 100, LayerMask.GetMask("TP")))
		{
			m_targetPosition = m_rayHit.collider.transform.position;
		}
	}

	public void Teleport(bool inputValue)
	{
		if(inputValue)
			MTK_Fade.Start(Color.black, m_fadeStart, MoveMtkManager);
	}

	private void MoveMtkManager()
	{
		m_mtkManager.transform.position = m_targetPosition;
		MTK_Fade.Start(Color.clear, m_fadeEnd);
	}
}
