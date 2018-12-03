using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RTPC_", menuName = "RTPC", order = 1)]
public class RTPC : ScriptableObject
{
	public string Name = "";

	[Header("Settings")]
	public float ValueMax;
	public int Multiplicator = 100;
	public int SnapshotsCount = 0;
	public bool UseAverage = false;

	float m_value;
	Queue<float> m_valueSnapshots;

	public float Value
	{
		get
		{
			return (UseAverage ? Average() : m_value) * Multiplicator;
		}
		set
		{
			m_value = value / ValueMax;

			if(SnapshotsCount != 0)
			{
				m_valueSnapshots.Enqueue(m_value);
				m_valueSnapshots.Dequeue();
			}

			AkSoundEngine.SetRTPCValue(Name,Value);
		}
	}

	public float Average()
	{
		float average = 0;

		foreach (var val in m_valueSnapshots)
			average += val;

		average /= SnapshotsCount;

		return average;
	}

	private void OnEnable()
	{
		m_valueSnapshots = new Queue<float>(new float[SnapshotsCount]);
	}
}