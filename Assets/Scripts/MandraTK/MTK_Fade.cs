using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_Fade : MonoBehaviour {

	public static MTK_Fade instance;

	protected Material fadeMaterial = null;
	protected Color currentColor = new Color(0f, 0f, 0f, 0f);
	protected Color targetColor = new Color(0f, 0f, 0f, 0f);
	protected Color deltaColor = new Color(0f, 0f, 0f, 0f);

	public void Fade(bool black)
	{
		StartFade(black ? Color.black : Color.clear, 0.2f);
	}

	public static void Start(Color newColor, float duration, VoidDelegate action = null)
	{
		if (instance)
		{
			instance.StartFade(newColor, duration, action);
		}
	}

	public virtual void StartFade(Color newColor, float duration, VoidDelegate action = null)
	{
		if (duration > 0.0f)
		{
			targetColor = newColor;
			deltaColor = (targetColor - currentColor) / duration;
			StartCoroutine(Fade(action));
		}
		else
		{
			currentColor = newColor;
		}
	}

	IEnumerator Fade(VoidDelegate action)
	{
		while (currentColor != targetColor)
		{
			if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.fixedDeltaTime)
			{
				currentColor = targetColor;
				deltaColor = new Color(0, 0, 0, 0);
			}
			else
			{
				currentColor += deltaColor * Time.fixedDeltaTime;
			}

			yield return new WaitForFixedUpdate();
		}
		
		if(action != null)
			action();
	}

	protected virtual void Awake()
	{
		fadeMaterial = new Material(Shader.Find("Unlit/TransparentColor"));
		instance = this;
	}

	protected virtual void OnPostRender()
	{
		if (currentColor.a > 0 && fadeMaterial)
		{
			currentColor.a = (targetColor.a > currentColor.a && currentColor.a > 0.98f ? 1f : currentColor.a);
			fadeMaterial.color = currentColor;
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, 0.9999f);
			GL.Vertex3(0f, 1f, 0.9999f);
			GL.Vertex3(1f, 1f, 0.9999f);
			GL.Vertex3(1f, 0f, 0.9999f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
