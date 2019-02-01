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

    MeshRenderer m_meshrenderer;
    private bool m_state = false;

    private void Start()
    {
        m_meshrenderer = GetComponent<MeshRenderer>();
        SetColorOn(false);
    }

    public void SetColorOn( bool state )
    {
        if (m_meshrenderer)
        {
            if ( state )
            {
                m_meshrenderer.material.color = m_colorOn;
            }
            else
            {

                m_meshrenderer.material.color = m_colorOff;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( ! m_state )
        {
            onButtonPressed.Invoke();
            wOnButtonReleased.Post(gameObject);
            m_state = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_state)
        {
            wOnButtonReleased.Post(gameObject);
            m_state = false;
        }
    }
}
