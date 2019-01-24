using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyEffector : MonoBehaviour
{
	[SerializeField] Vector3 m_force;
	[SerializeField] float m_accel;
    [SerializeField] List<Rigidbody> m_rgbdList = new List<Rigidbody>();
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
			if(rgbd)
			{
				distance = Mathf.Sqrt(1 - Mathf.Abs((rgbd.position.y - transform.position.y) / (m_collider.size.y / 2))) * 2;

				if(distance > 0)
					rgbd.velocity = Vector3.Lerp(rgbd.velocity, m_force, Time.fixedDeltaTime * distance * m_accel);
			}
			else
			{
				m_rgbdList.Remove(rgbd);
				break;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!m_rgbdList.Contains(other.attachedRigidbody) && other.attachedRigidbody)
			m_rgbdList.Add(other.attachedRigidbody);
	}

	private void OnTriggerExit(Collider other)
	{
		m_rgbdList.Remove(other.attachedRigidbody);
	}
}
