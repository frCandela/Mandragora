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

    public MTK_Interactable catchedObject { get { return m_catchedObject; } }

    private MTK_Interactable m_catchedObject;
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
        if(m_nbObjectsInTrigger > 0 && !m_catchedObject)
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
        m_catchedObject = null;
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
            if(m_snapToCenter)
            {
                interactable.transform.position = transform.position;
            }

            interactable.jointType.JoinWith(gameObject);
            m_catchedObject = interactable;
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
        MTK_Interactable interact = other.GetComponent<MTK_Interactable>();
        if (interact)
        {
            m_nbObjectsInTrigger++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_Interactable interact = other.GetComponent<MTK_Interactable>();
        if (interact)
        {
            m_nbObjectsInTrigger--;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        MTK_Interactable interactable = other.GetComponent<MTK_Interactable>();
        if(interactable)
        {
            Catch(interactable);            
        }
    }

}
