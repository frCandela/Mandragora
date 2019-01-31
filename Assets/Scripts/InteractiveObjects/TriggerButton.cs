using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : MonoBehaviour
{
    [SerializeField] private Color m_colorOn = Color.green;
    [SerializeField] private Color m_colorOff = Color.red;

    [SerializeField] public UnityEvent onButtonPressed;
    [SerializeField] public AK.Wwise.Event wOnButtonReleased;

    private bool m_state = false;

    private void Start()
    {
        SetColorOn(false);
    }

    public void SetColorOn( bool state )
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr)
        {
            if ( state )
            {
                mr.material.color = m_colorOn;
            }
            else
            {

                mr.material.color = m_colorOff;
                wOnButtonReleased.Post(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( ! m_state )
        {
            onButtonPressed.Invoke();
            m_state = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_state)
        {
            m_state = false;
        }
    }
}
