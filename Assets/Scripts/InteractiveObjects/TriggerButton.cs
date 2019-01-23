using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : MonoBehaviour
{
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    [SerializeField] private bool m_changeColor = false;

    private bool m_state = false;

    private void OnTriggerEnter(Collider other)
    {
        if( ! m_state )
        {
            onButtonPressed.Invoke();
            m_state = true;


            if(m_changeColor)
            {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if (mr)
                {
                    mr.material.color = Color.red;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_state)
        {
            onButtonReleased.Invoke();
            m_state = false;


            if (m_changeColor)
            {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if (mr)
                {
                    mr.material.color = Color.white;
                }
            }
        }
    }
}
