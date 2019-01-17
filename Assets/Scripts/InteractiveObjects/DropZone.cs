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

    public UnityEventBool onObjectCatched;
    public MTK_Interactable catchedObject { get; private set; }

    private Outline m_outline;
    MeshRenderer m_meshRenderer;

    private int m_nbObjectsInTrigger = 0;
    private float m_lastActivationTime;

    // Use this for initialization
    void Awake ()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;

        m_lastActivationTime = Time.time;
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

    public void Release()
    {        
        if (catchedObject)
        {
            m_lastActivationTime = Time.time;
            m_meshRenderer.enabled = true;
            onObjectCatched.Invoke(false);

            MTK_Interactable tmp = catchedObject;
            catchedObject = null;

            tmp.jointType.RemoveJoint();
            tmp.GetComponent<Rigidbody>().AddForce(m_ejectForce * Vector3.up, ForceMode.Impulse);
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
                catchedObject = interactable;
                interactable.jointType.onJointBreak.AddListener(Release);

                onObjectCatched.Invoke(true);
                SetRtpc(2);

                if (m_meshRenderer)
                {
                    m_meshRenderer.enabled = false;
                }
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

    [ContextMenu("TestRTPC")]
    void TestRTPC()
    {
        StartCoroutine(SetRtpc(5));
    }

    IEnumerator SetRtpc(float T)
    {
        for (float i = 1; i > 0; i -= Time.deltaTime / T)
        {
            print(i);
            AkSoundEngine.SetRTPCValue("Diegetic", i * 100);
            yield return new WaitForEndOfFrame();
        }
    }
}
