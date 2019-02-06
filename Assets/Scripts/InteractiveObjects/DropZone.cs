using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
public class DropZone : MonoBehaviour
{    
    [SerializeField] private bool m_snapToCenter = true;
    [SerializeField] private float m_activationCooldown = 2f;
    [SerializeField] private float m_ejectForce = 1f;
    [SerializeField] private Animator m_workshopAnimator;
    [SerializeField] private SocleSounds m_sounds;
    private GameObject m_visual;

    public UnityEventBool onObjectCatched;
    public MTK_Interactable catchedObject { get; private set; }


    private Outline m_outline;
    [SerializeField] TriggerButton m_button;

    private int m_nbObjectsInTrigger = 0;
    private float m_lastActivationTime;

    // Use this for initialization
    void Awake ()
    {
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;

        m_lastActivationTime = Time.time;

        m_visual = transform.Find("dropZone_Final").gameObject;

        onObjectCatched.AddListener((bool catched) =>
        {
            m_workshopAnimator.SetBool("isOn", catched);
            m_sounds.State = catched;
        });
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(m_nbObjectsInTrigger > 0 && !catchedObject)
        {
            m_outline.enabled = true;
        }
        else
        {
            m_outline.enabled = false;
        }        
    }

    public void EnableButton()
    {
        m_button.SetActive(true);
    }

    public void Release()
    {        
        if (catchedObject)
        {
            m_lastActivationTime = Time.time;
            onObjectCatched.Invoke(false);
            catchedObject.jointType.onJointBreak.RemoveListener(Release);

            MTK_Interactable tmp = catchedObject;
            catchedObject = null;

            IcoPlanet icoplanet = tmp.GetComponent<IcoPlanet>();

            if(icoplanet)
                icoplanet.Joined = false;

            tmp.jointType.RemoveJoint();
            tmp.GetComponent<Rigidbody>().AddForce(m_ejectForce * Vector3.up, ForceMode.Impulse);

            m_visual.SetActive(true);
            m_nbObjectsInTrigger = 0;

            m_button.SetActive(false);
        }
    }

    private void OnJointBreak(float breakForce)
    {
        Release();
    }

    void Catch(MTK_Interactable interactable)
    {
        if(Time.time > m_lastActivationTime + m_activationCooldown)
        {
            if (!interactable.jointType.Used() && !catchedObject && !interactable.isDistanceGrabbed)
            {
                m_lastActivationTime = Time.time;
                if (m_snapToCenter)
                {
                    interactable.transform.position = transform.position;
                }

                interactable.jointType.JoinWith(gameObject);
                GetComponent<Joint>().breakForce = float.MaxValue;
                AkSoundEngine.PostEvent("Socle_Activated_Play", gameObject);
                catchedObject = interactable;
                interactable.jointType.onJointBreak.AddListener(Release);

                IcoPlanet icoplanet = interactable.GetComponent<IcoPlanet>();

                if(icoplanet)
                    icoplanet.Joined = true;

                m_visual.SetActive(false);
                m_nbObjectsInTrigger = 0;

                onObjectCatched.Invoke(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            MTK_Interactable interact = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interact && interact.isDroppable)
            {
                m_nbObjectsInTrigger++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody)
        {
            MTK_Interactable interact = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interact && interact.isDroppable)
            {
                m_nbObjectsInTrigger--;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            MTK_Interactable interactable = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interactable && interactable.isDroppable)
            {
                Catch(interactable);
            }
        }
    }
}
