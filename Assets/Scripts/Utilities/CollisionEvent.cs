using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] public enum CollisionType { Collision, Trigger, All }
    [SerializeField] private CollisionType m_collisionType = CollisionType.All, m_otherColliderType;

    [SerializeField] UnityEvent m_onEnter, m_onExit;


	private void OnCollisionEnter(Collision other)
	{
        if(m_collisionType == CollisionType.All || m_collisionType == CollisionType.Collision)
			if(CheckOtherCollider(other.collider))
		    	m_onEnter.Invoke();
	}

	private void OnTriggerEnter(Collider other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Trigger)
			if(CheckOtherCollider(other))
            	m_onEnter.Invoke();
	}

	private void OnCollisionExit(Collision other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Collision)
			if(CheckOtherCollider(other.collider))
            	m_onExit.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
        if (m_collisionType == CollisionType.All || m_collisionType == CollisionType.Trigger)
			if(CheckOtherCollider(other))
            	m_onExit.Invoke();
	}

	bool CheckOtherCollider(Collider other)
	{
		if(m_otherColliderType == CollisionType.All)
			return true;

		if(m_otherColliderType == CollisionType.Trigger && other.isTrigger)
			return true;

		if(m_otherColliderType == CollisionType.Collision && !other.isTrigger)
			return true;

		return false;
	}
}
