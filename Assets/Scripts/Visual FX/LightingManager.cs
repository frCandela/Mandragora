using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour {

	[Range(0.0f, 1.0f)] public float sceneUnlitFactor;
	public GameObject sunGO;
	public bool isSunVisible = true;
	public List<Light> lights;
	public bool lightsOn = true;
	private MeshRenderer sunRenderer;

	// Use this for initialization
	void Start () {
		sunRenderer = sunGO.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalFloat("_ManagerUnlitFactor", sceneUnlitFactor);

		if(!lightsOn) {
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
		sunGO.SetActive(isSunVisible);
	}
}
