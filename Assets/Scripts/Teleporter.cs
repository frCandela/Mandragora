using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	[SerializeField] MTK_Manager m_mtkManager;
	RaycastHit m_rayHit;

	Vector3 m_targetPosition;
	
	void Update ()
	{
		Transform origin =	m_mtkManager.activeSetup.head.transform;

		if(Physics.Raycast(origin.position, origin.forward, out m_rayHit, 100, LayerMask.GetMask("TP")))
		{
			m_targetPosition = m_rayHit.collider.transform.position;
		}
	}

	public void Teleport()
	{
		m_mtkManager.transform.position = m_targetPosition;
	}
}
