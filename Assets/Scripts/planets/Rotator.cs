using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.right;
    [SerializeField] private float angularSpeed = 1;

    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(angularSpeed * axis * Time.deltaTime);
	}
}
