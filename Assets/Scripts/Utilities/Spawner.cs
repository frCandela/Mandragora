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
		Spawn(transform.position, transform.rotation, null);
	}

	protected void Spawn(Vector3 position, Quaternion rotation, Transform parent)
	{
		m_spawned = Instantiate(m_object, position, rotation, parent);
	}

	public virtual void SpawnIntParameter(int parameter){}
}
