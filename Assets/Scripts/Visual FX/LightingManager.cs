using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class LightingManager : MonoBehaviour {

	[Range(0.0f, 1.0f)] public float sceneUnlitFactor;
	[SerializeField] MeshRenderer zoneRenderer;
	[SerializeField] List<MonoBehaviour> m_monoList;

	bool vibrate = false;

	// Use this for initialization
	void Awake ()
	{
		foreach (MonoBehaviour mono in m_monoList)
			mono.enabled = false;
	}

	private void Start()
	{
		if(zoneRenderer)
		{
			zoneRenderer.enabled = false;
		}
	}

	MTK_InputManager[] m_inputManagers;
	void TriggerSound()
	{
		AkSoundEngine.PostEvent("Sun_Light_Play", gameObject);
		vibrate = true;

		m_inputManagers = FindObjectsOfType<MTK_InputManager>();
	}

	void PlaySunExplosion()
	{
		AkSoundEngine.PostEvent("Play_Sun_Explosion", gameObject);
	}

	void ActivateControl()
	{
		foreach (MonoBehaviour mono in m_monoList)
		{
			mono.enabled = true;
		}

		if(zoneRenderer)
		{
			zoneRenderer.enabled = true;
		}

		vibrate = false;
	}
	
	// Update is called once per frame
	[ContextMenu("Update")]
	void Update () {
		Shader.SetGlobalFloat("_ManagerUnlitFactor", sceneUnlitFactor);

		if(vibrate)
		{
			foreach (MTK_InputManager inputmng in m_inputManagers)
			{
				inputmng.Haptic(1);
			}
		}

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
