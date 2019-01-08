using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinder : MTK_Interactable
{
	[SerializeField] GameObject m_planet;

	bool m_enabled;

	void Activate()
	{
		isDistanceGrabbable = true;
		m_enabled = true;
	}

	void Break()
	{
		m_planet.GetComponent<Rigidbody>().isKinematic = false;
		m_planet.GetComponent<Collider>().enabled = true;
		m_planet.GetComponent<MTK_Interactable>().isDistanceGrabbable = true;

		foreach (Transform child in transform)
			child.transform.SetParent(transform.parent,true);

		Destroy(gameObject);
	}

	public override void Grab(bool input)
	{
		GetComponent<Rigidbody>().isKinematic = false;

		base.Grab(input);
	}

	private void OnCollisionEnter(Collision other)
	{
		if(m_enabled)
			GetComponent<Rigidbody>().isKinematic = false;

		if(other.gameObject.CompareTag("KinderBreaker"))
			Break();
	}
}
