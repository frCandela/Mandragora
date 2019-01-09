using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
public class DropZone : MonoBehaviour
{    
    [SerializeField] private bool m_snapToCenter = true;

    public UnityEventBool onObjectCatched;

    public MTK_Interactable catchedObject { get; private set; }

    private Outline m_outline;
    private int m_nbObjectsInTrigger = 0;
    MeshRenderer m_meshRenderer ;

    // Use this for initialization
    void Awake ()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;
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

    void Release()
    {
        catchedObject = null;
        if (m_meshRenderer)
        {
            m_meshRenderer.enabled = true;
            onObjectCatched.Invoke(false);
        }
    }

    void Catch(MTK_Interactable interactable)
    {
        if (!interactable.jointType.Used())
        {
            print("Catch" + interactable.name);
            if (m_snapToCenter)
            {
                interactable.transform.position = transform.position;
            }

            interactable.jointType.JoinWith(gameObject);
            catchedObject = interactable;
            interactable.jointType.onJointBreak.AddListener(Release);

            onObjectCatched.Invoke(true);

            if (m_meshRenderer)
            {
                m_meshRenderer.enabled = false;
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
