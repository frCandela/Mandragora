using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : MonoBehaviour
{
    [SerializeField] public UnityEvent onButtonPressed;
    static float m_timeToTrigger = .5f;

    private bool m_state = false;
    private Animator m_animator;
    private Collider m_collider;

    MTK_InputManager m_currentController;

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

            if(value)
                AkSoundEngine.PostEvent("Play_Load_Button_Off", gameObject);
            else
                AkSoundEngine.PostEvent("Stop_Load_Button_Off", gameObject);

            if(m_state)
                m_blend = 0;

            m_state = value;
        }
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
    }

    public void SetActive(bool value)
    {
        if(value)
            AkSoundEngine.PostEvent("Play_Button_ON", gameObject);

        m_animator.SetBool("Active", value);
        m_collider.enabled = value;
    }

    private void Update()
    {
        m_animator.SetFloat("blend", m_blend);
        m_blend = Mathf.Clamp01(m_blend + Time.deltaTime * (State ? 1 : -1) / m_timeToTrigger);

        if(State)
        {
            m_currentController.Haptic(.1f);

            if(m_blend == 1)
            {
                AkSoundEngine.PostEvent("Button_Play", gameObject);
                onButtonPressed.Invoke();
                State = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( ! State )
            State = true;

        m_currentController = other.GetComponentInParent<MTK_InputManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (State)
            State = false;
    }
}
