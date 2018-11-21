using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    [SerializeField] private float forwardForce = 1;

    private Rigidbody m_rb;

	// Use this for initialization
	void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        float forwardAxis = 0f;   
        if (Input.GetKey(KeyCode.Z))
        {
            forwardAxis += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forwardAxis -= 1f;
        }
        m_rb.AddForce(forwardAxis * forwardForce * forward, ForceMode.VelocityChange);


        float rightAxis = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            rightAxis += 1f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rightAxis -= 1f;
        }
        m_rb.AddForce(rightAxis * forwardForce * right, ForceMode.VelocityChange);
    }
}
