using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MTK_InertJoint))]
[RequireComponent(typeof(MTK_Interactable))]
public class MTK_Potentiometer : MonoBehaviour {

    [SerializeField] private float m_angle = 0f;
    [SerializeField] private Vector3 m_axis = Vector3.up;
    [SerializeField] private Vector3 m_handAxis = Vector3.forward;

    private MTK_JointType m_joint;
    private MTK_Interactable m_interact;

    // Use this for initialization
    void Awake ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
