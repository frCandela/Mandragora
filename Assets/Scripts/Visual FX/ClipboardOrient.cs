using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardOrient : MonoBehaviour {

	public List<Transform> clipPlanes;

	
	// Update is called once per frame
	void Update () {

			foreach(Transform item in clipPlanes)
			{
				if(item != null) {
					item.LookAt(Camera.main.transform, Vector3.up);
				}
			}
		
	}
}
