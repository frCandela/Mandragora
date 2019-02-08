using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSound : MonoBehaviour
{
	public float m_position;
	PathCreation.PathCreator m_path;

	private void Start()
	{
		m_path = GetComponentInParent<PathCreation.PathCreator>();
	}

	private void Update()
	{
		transform.position = m_path.path.GetPointAtDistance(m_position * m_path.path.length, PathCreation.EndOfPathInstruction.Stop);
	}

	private void OnEnable()
	{
		AkSoundEngine.PostEvent("Play_Pipe", gameObject);
	}

	private void OnDisable()
	{
		AkSoundEngine.PostEvent("Stop_Pipe", gameObject);
	}
}
