using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateSelf : MonoBehaviour {

	public Vector3 rotationAxis;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		this.transform.Rotate(rotationAxis, Space.Self);
		
	}
}
