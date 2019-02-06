using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
	Dictionary<string, Transform> m_effectsList;

	void Awake()
	{
		m_effectsList = new Dictionary<string, Transform>(transform.childCount);

		foreach (Transform child in transform)
			m_effectsList.Add(child.name, child);
	}
	
	public void Activate(string effectName, Transform target)
	{
		Transform currentEffect = m_effectsList[effectName];

		if(effectName == "Grab")
			currentEffect.SetParent(target, true);
			
		currentEffect.position = target.position;
		currentEffect.GetComponent<ParticleSystem>().Play();

		AkEvent wwiseEvent = currentEffect.GetComponent<AkEvent>();
		if(wwiseEvent)
			wwiseEvent.HandleEvent(currentEffect.gameObject);
	}

	public void DeActivate(string effectName)
	{
		Transform currentEffect = m_effectsList[effectName];

		currentEffect.SetParent(transform);
		currentEffect.GetComponent<ParticleSystem>().Stop();
	}
}
