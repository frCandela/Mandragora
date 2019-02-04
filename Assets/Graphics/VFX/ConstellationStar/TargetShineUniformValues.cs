using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class TargetShineUniformValues : MonoBehaviour {

	public float radius = 1.0f;
	
	// Update is called once per frame
	void Update () {

		Shader.SetGlobalFloat("_TargetRadius", radius);
		Shader.SetGlobalVector("_TargetWorldPosition", (Vector4)transform.position);
		
	}
}
