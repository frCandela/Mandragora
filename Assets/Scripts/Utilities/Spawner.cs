using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	GameObject m_object;

	protected GameObject m_spawned;

	public void Spawn()
	{
		m_spawned = Instantiate(m_object, transform.position, transform.rotation);
	}

	public virtual void SpawnIntParameter(int parameter){}
}
