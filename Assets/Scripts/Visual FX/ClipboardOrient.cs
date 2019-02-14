using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardOrient : MonoBehaviour {

	public List<Transform> clipPlanes;
	public Transform target;


	
	// Update is called once per frame
	void Update () {

			foreach(Transform item in clipPlanes)
			{
				if(item != null) {
					item.LookAt(target.transform, Vector3.up);
				}
			}
		
	}
}
