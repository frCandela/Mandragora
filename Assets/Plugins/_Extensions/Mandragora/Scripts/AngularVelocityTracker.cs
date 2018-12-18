using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityTracker : MonoBehaviour
{
	Quaternion lastRot;
	List<Vector3> rotList;
	float magnitude;
	Vector3 axis;

	public Vector3 GetAngularVelocity()
	{
		Vector3 angularVelocity = Vector3.zero;
		foreach (Vector3 vel in rotList)
		{
			angularVelocity += vel;
		}
		angularVelocity /= rotList.Count;
		return angularVelocity;
	}

	void Awake()
	{
		rotList = new List<Vector3>();
		lastRot = transform.rotation;
	}

	void Update()
	{
		Quaternion deltaRotation = transform.rotation * Quaternion.Inverse (lastRot);
		deltaRotation.ToAngleAxis(out magnitude, out axis);
		rotList.Add((axis * magnitude));

		if (rotList.Count > 4)
			rotList.RemoveAt(0);
			
		lastRot = transform.rotation;
	}
}
