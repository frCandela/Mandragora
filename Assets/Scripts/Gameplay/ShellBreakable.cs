using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBreakable : MonoBehaviour
{

	
	private void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "KinderBreaker")
		{
			Break();
		}
	}

	void Break()
	{
		Destroy(gameObject);
	}
}
