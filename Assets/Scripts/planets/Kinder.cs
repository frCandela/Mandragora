using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinder : MTK_Interactable
{
	[SerializeField] GameObject m_planet;
	[SerializeField] Transform m_shell;
	[SerializeField] AK.Wwise.Event m_kinderCreation;
	[SerializeField] float m_minBreakMagnitude;
	[SerializeField, Range(0,1f)] float m_breakThreshold = .4f;
	[SerializeField] ParticleSystem m_hitPs, m_breakPs;

	bool m_enabled;
	Rigidbody m_rgbd;
	int m_piecesCount;

	protected override void OnEnable()
	{
		m_rgbd = GetComponent<Rigidbody>();

		m_piecesCount = (int) (m_shell.childCount * m_breakThreshold);
	}

	void Activate()
	{
		base.OnEnable();
		isGrabbable = true;
		m_enabled = true;
		m_rgbd.isKinematic = false;
	}

	public void TriggerKinderSound()
	{
		foreach (MeshCollider c in m_planet.GetComponents<MeshCollider>())
			c.enabled = false;

		m_kinderCreation.Post(gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		if(other.relativeVelocity.sqrMagnitude > m_minBreakMagnitude)
		{
			other.contacts[0].thisCollider.gameObject.SetActive(false);
			m_hitPs.transform.position = other.contacts[0].point;
			m_hitPs.transform.LookAt(other.contacts[0].point + other.contacts[0].normal);
			m_hitPs.Emit(Random.Range(1,3));

			AkSoundEngine.SetRTPCValue("Force", other.relativeVelocity.sqrMagnitude);
			AkSoundEngine.PostEvent("Kinder_Break_Play", gameObject);

			m_piecesCount--;

			if(m_piecesCount == 0)
				Break();
		}
	}

	int m_needFix = 0;
	private void FixedUpdate()
	{
		if(m_needFix > 0)
		{
			m_planet.GetComponent<MeshCollider>().enabled = !m_planet.GetComponent<MeshCollider>().enabled;
			m_planet.GetComponent<Rigidbody>().isKinematic = !m_planet.GetComponent<MeshCollider>().enabled;
			m_needFix--;
		}
	}

	[ContextMenu("Break")]
	void Break()
	{
		m_needFix = 101;

		m_planet.transform.SetParent(transform.parent, true);

		m_planet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        m_planet.GetComponent<MTK_Interactable>().enabled = true;

		foreach (Transform tr in m_planet.transform)
			tr.localScale = Vector3.one;

		Destroy(m_rgbd);
		Destroy(GetComponent<Collider>());
		m_shell.gameObject.SetActive(false);
		m_breakPs.gameObject.SetActive(true);

		AkSoundEngine.PostEvent("Kinder_Break_Play", gameObject);

		Destroy(gameObject, 10);
	}
}
