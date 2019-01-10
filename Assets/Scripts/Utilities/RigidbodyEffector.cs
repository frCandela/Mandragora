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
			Vector3 forceToApply = Time.fixedDeltaTime * m_force * distance;

			if(rgbd.velocity.y < m_maxVel.y && forceToApply.magnitude > 0)
				rgbd.AddForce(forceToApply);

			rgbd.velocity = Vector3.MoveTowards(rgbd.velocity, new Vector3(0,rgbd.velocity.y,0), Time.fixedDeltaTime * Mathf.Abs(rgbd.velocity.x * rgbd.velocity.z));
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
