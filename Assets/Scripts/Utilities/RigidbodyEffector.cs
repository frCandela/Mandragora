using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyEffector : MonoBehaviour
{
	[SerializeField] Vector3 m_force;
	[SerializeField] Vector3 m_maxVel;
	List<Rigidbody> m_rgbdList = new List<Rigidbody>();
	BoxCollider m_collider;

	private void Start()
	{
		m_collider = GetComponent<BoxCollider>();
	}
	
	void FixedUpdate ()
	{
		float distance;

		foreach (Rigidbody rgbd in m_rgbdList)
		{
			distance = Mathf.Sqrt(1 - Mathf.Abs((rgbd.position.y - transform.position.y) / (m_collider.size.y / 2)));

			if(rgbd.velocity.y < m_maxVel.y)
				rgbd.AddForce(m_force * distance);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		m_rgbdList.Add(other.attachedRigidbody);
	}

	private void OnTriggerExit(Collider other)
	{
		m_rgbdList.Remove(other.attachedRigidbody);
	}
}
