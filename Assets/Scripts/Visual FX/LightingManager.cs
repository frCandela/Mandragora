using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class LightingManager : MonoBehaviour {

	[Range(0.0f, 1.0f)] public float sceneUnlitFactor;
	//public GameObject sunGO;
	//public bool isLit = true;
	//public List<Light> lights;
	private MeshRenderer sunRenderer;

	List<MonoBehaviour> m_monoList;

	// Use this for initialization
	void Awake () {
		//sunRenderer = sunGO.GetComponent<MeshRenderer>();

		m_monoList = new List<MonoBehaviour>();

		m_monoList.AddRange(FindObjectsOfType<TelekinesisPointer>());
		m_monoList.AddRange(FindObjectsOfType<Teleporter>());

		foreach (MonoBehaviour mono in m_monoList)
			mono.enabled = false;
	}

	void TriggerSound()
	{
		AkSoundEngine.PostEvent("Sun_Light_Play", gameObject);
	}

	void ActivateControl()
	{
		foreach (MonoBehaviour mono in m_monoList)
			mono.enabled = true;
	}
	
	// Update is called once per frame
	[ContextMenu("Update")]
	void Update () {
		Shader.SetGlobalFloat("_ManagerUnlitFactor", sceneUnlitFactor);

		/*if(!isLit) {
			foreach(Light item in lights)
			{
				item.enabled = false;
			}
		} else {
			foreach(Light item in lights)
			{
				item.enabled = true;
			}
		}

		//sunRenderer.material.SetFloat("_Visibility", sunVisibility);
		sunGO.SetActive(isLit);*/
	}
}
