using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_InputManagerSimulator : MTK_InputManager
{
    // public enum Hand {left, right}
    // [SerializeField] private Hand m_hand;

    Quaternion m_lastRot;
	List<Vector3> m_rotList;
	float m_magnitude;
	Vector3 m_axis;

	List<Vector3> m_posList;

    void Awake()
	{
		m_rotList = new List<Vector3>(5);
		m_posList = new List<Vector3>(5);
		m_lastRot = transform.rotation;
	}

    void Update()
    {
        // Manage inputs
        if (Input.GetMouseButtonDown(0))
            onInput.Invoke(InputButtons.Grip, true);
        if (Input.GetMouseButtonUp(0))
            onInput.Invoke(InputButtons.Grip,false);
            
        if (Input.GetMouseButtonDown(1))
            onInput.Invoke(InputButtons.Trigger, true);
        if (Input.GetMouseButtonUp(1))
            onInput.Invoke(InputButtons.Trigger, false);

        if (Input.GetMouseButtonDown(1))
            onInput.Invoke(InputButtons.Pad, true);
        if (Input.GetMouseButtonUp(1))
            onInput.Invoke(InputButtons.Pad, false);

        // Update Angular Velocity
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse (m_lastRot);
		deltaRotation.ToAngleAxis(out m_magnitude, out m_axis);
		m_rotList.Add((m_axis * m_magnitude));

		if (m_rotList.Count > 4)
			m_rotList.RemoveAt(0);
			
		m_lastRot = transform.rotation;

        // Update Velocity
        m_posList.Add(transform.position);

        if (m_posList.Count > 4)
			m_posList.RemoveAt(0);
    }

    public override void Haptic(float Time)
    {
        print("bzz bzz");
    }

    public override Vector3 GetAngularVelocity()
    {
        Vector3 angularVelocity = Vector3.zero;

		foreach (Vector3 vel in m_rotList)
			angularVelocity += vel;

		angularVelocity /= m_rotList.Count;
		return angularVelocity;
    }

    public override Vector3 GetVelocity()
    {
        Vector3 velocity = Vector3.zero;

        foreach (Vector3 vel in m_posList)
			velocity += vel;
            
		velocity /= m_posList.Count;

        return velocity;
    }
}
