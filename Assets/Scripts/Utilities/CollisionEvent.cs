using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
	[SerializeField]
	UnityEvent m_onEnter, m_onExit;

	private void OnCollisionEnter(Collision other)
	{
		m_onEnter.Invoke();
	}

	private void OnTriggerEnter(Collider other)
	{
		m_onEnter.Invoke();
	}

	private void OnCollisionExit(Collision other)
	{
		m_onExit.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		m_onExit.Invoke();
	}
}
