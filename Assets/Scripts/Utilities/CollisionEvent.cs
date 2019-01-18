using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] public enum CollisionType { Collision, Trigger, All }
    [SerializeField] private CollisionType m_collisionType = CollisionType.All;

    [SerializeField] UnityEvent m_onEnter, m_onExit;


	private void OnCollisionEnter(Collision other)
	{
        if(m_collisionType == CollisionType.All || m_collisionType == CollisionType.Collision)
		    m_onEnter.Invoke();
	}

	private void OnTriggerEnter(Collider other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Trigger)
            m_onEnter.Invoke();
	}

	private void OnCollisionExit(Collision other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Collision)
            m_onExit.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Trigger)
            m_onExit.Invoke();
	}
}
