using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour {

	[Range(0.0f, 1.0f)] public float sceneUnlitFactor;
	[SerializeField] MeshRenderer zoneRenderer;
	[SerializeField] List<MonoBehaviour> m_monoList;

	[SerializeField] Animator[] m_soclesAnimators;

	bool vibrate = false;

    // Use this for initialization
    void Awake()
    {
        foreach (MonoBehaviour mono in m_monoList)
        {
            if (mono)
                mono.enabled = false;
        }
    }

	private void Start()
	{
		AkSoundEngine.PostEvent("Mute_Drop", gameObject);
		AkSoundEngine.PostEvent("Play_Intro", gameObject);

		if(zoneRenderer)
		{
			zoneRenderer.enabled = false;
		}
	}

	MTK_InputManager[] m_inputManagers;
	void TriggerSound()
	{
		AkSoundEngine.PostEvent("Stop_Intro", gameObject);
		AkSoundEngine.PostEvent("Sun_Light_Play", gameObject);
		vibrate = true;

		m_inputManagers = FindObjectsOfType<MTK_InputManager>();
	}

	void PlaySunExplosion()
	{
		AkSoundEngine.PostEvent("Play_Sun_Explosion", gameObject);
		AkSoundEngine.PostEvent("UnMute_Drop", gameObject);
		AkSoundEngine.PostEvent("Music_Play", gameObject);

		foreach (Animator animator in m_soclesAnimators)
			animator.enabled = true;
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

	public void DeActivateControl()
	{
		foreach (MonoBehaviour mono in m_monoList)
		{
			mono.enabled = false;
		}

		if(zoneRenderer)
		{
			zoneRenderer.enabled = false;
		}
	}
	
	float m_vibrationIntensity = 0;
	// Update is called once per frame
	[ContextMenu("Update")]
	void Update () {
		Shader.SetGlobalFloat("_ManagerUnlitFactor", sceneUnlitFactor);

		if(vibrate)
		{
			m_vibrationIntensity += Time.deltaTime /20;

			foreach (MTK_InputManager inputmng in m_inputManagers)
				inputmng.Haptic(m_vibrationIntensity);
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
