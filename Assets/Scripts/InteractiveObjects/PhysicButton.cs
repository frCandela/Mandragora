using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicButton : MonoBehaviour
{

    [SerializeField] private GameObject buttonGameObject;
    [SerializeField] private bool debugColor = true;

    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private bool m_state = false;


    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject == buttonGameObject)
        {
            if( m_state )
            {
                onButtonReleased.Invoke();
            }

            m_state = false;

            if(debugColor)
            {
                GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == buttonGameObject)
        {
            if ( ! m_state)
            {
                onButtonPressed.Invoke();
            }

            m_state = true;

            if (debugColor)
            {
                GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }

}

