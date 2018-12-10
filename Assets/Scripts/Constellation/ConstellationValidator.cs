using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationValidator : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		ConstellationStar star = other.GetComponent<ConstellationStar>();

		if(star)
		{
			star.Validated = true;
		}
	}
}
