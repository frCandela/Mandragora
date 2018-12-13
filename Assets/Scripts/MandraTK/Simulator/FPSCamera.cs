using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    [SerializeField] RotationAxes axes = RotationAxes.MouseXAndY;
    [SerializeField] KeyCode m_crouch;
    [SerializeField] float sensitivityX = 15F;
    [SerializeField] float sensitivityY = 15F;
    [SerializeField] float minimumX = -360F;
    [SerializeField] float maximumX = 360F;
    [SerializeField] float minimumY = -60F;
    [SerializeField] float maximumY = 60F;
    [SerializeField] float rotationY = 0F;

    float m_baseHeight;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_baseHeight = transform.position.y;
    }

    public CursorLockMode test;
    void Update()
    {

        test = Cursor.lockState;
        // Move camera
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }

        // Handle Crouch
        transform.localPosition = Vector3.up * (Input.GetKey(m_crouch) ? .8f : m_baseHeight);
    }
}
