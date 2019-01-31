using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInFrontOfCamera : MonoBehaviour {

	public Vector3 localPosOffset;
	public Vector3 localRotOffset;

	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = Camera.main.transform.position;
		this.transform.Translate(localPosOffset, Space.Self);
		//this.transform.rotation *= Quaternion.Euler(localRotOffset);
	}
}
