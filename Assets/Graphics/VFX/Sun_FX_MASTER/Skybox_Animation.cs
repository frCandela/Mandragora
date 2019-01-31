using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox_Animation : MonoBehaviour {

	public Material skyboxMat;
	public float exposure;
	private float exposureDefault;

	// Use this for initialization
	void Start () {

		skyboxMat = Camera.main.GetComponent<Skybox>().material;
		exposureDefault = skyboxMat.GetFloat("Exposure");
		exposure = exposureDefault;

	}

	void Update () {

		skyboxMat.SetFloat("Exposure", exposure);

	}
	
}
