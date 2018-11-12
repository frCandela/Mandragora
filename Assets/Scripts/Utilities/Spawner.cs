using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
	[SerializeField]
	GameObject m_object;
	
	public GameObject Spawn()
	{
		return Instantiate(m_object, transform.position, transform.rotation, transform);
	}

	public virtual void SpawnIntParameter(int parameter){}
}
