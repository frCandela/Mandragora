using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationValidator : MonoBehaviour
{
	MTK_InputManager m_inputManager;

	private void OnEnable()
	{
		m_inputManager = GetComponentInParent<MTK_InputManager>();
	}

	private void OnTriggerEnter(Collider other)
	{
		ConstellationStar star = other.GetComponent<ConstellationStar>();

		if(star)
			star.TryValidate(m_inputManager.GetVelocity());
	}
}
