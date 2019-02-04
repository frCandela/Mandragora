using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : MonoBehaviour
{
    [SerializeField] public UnityEvent onButtonPressed;
    [SerializeField] public AK.Wwise.Event wOnButtonReleased;
    static float m_timeToTrigger = 1;

    private bool m_state = false;
    private Animator m_animator;

    float m_blend;

    bool State
    {
        get
        {
            return m_state;
        }
        set
        {
            m_animator.SetBool("isCharging", true);
            wOnButtonReleased.Post(gameObject);

            if(m_state)
                m_blend = 0;

            m_state = value;
        }
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetActive(bool value)
    {
        m_animator.SetBool("Active", value);
    }

    private void Update()
    {
        m_animator.SetFloat("blend", m_blend);
        m_blend = Mathf.Clamp01(m_blend + Time.deltaTime * (State ? 1 : -1) / m_timeToTrigger);

        if(State && m_blend == 1)
        {
            onButtonPressed.Invoke();
            State = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( ! State )
            State = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (State)
            State = false;
    }
}
