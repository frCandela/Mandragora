using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessApply : MonoBehaviour {

	public Material postEffectMaterial;
	
	void OnRenderImage (RenderTexture src, RenderTexture dst) {
		
		Graphics.Blit(src, dst, postEffectMaterial);
	}
}
