using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class PlanetTerraformer : MonoBehaviour
{
    private DropZone m_dropzone;
    private IcoPlanet m_icoPlanet;

    // Use this for initialization
    void Start ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableTerraformation);
    }

    void EnableButton()
    {
        m_dropzone.EnableButton();
    }
	
	// Update is called once per frame
    void EnableTerraformation( bool state )
    {
        if( state )
        {
            m_icoPlanet = m_dropzone.catchedObject.GetComponent<IcoPlanet>();
            m_icoPlanet.Animate();

            Invoke("EnableButton", 2);

            if (m_icoPlanet)
            {
                MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();
                interactable.isDistanceGrabbable = false;
                interactable.isGrabbable = false;

                m_icoPlanet.GetComponent<Collider>().enabled = false;

                foreach ( IcoSegment segment in m_icoPlanet.Segments)
                {
                    segment.GetComponent<MTK_Interactable>().isGrabbable = true;
                    segment.GetComponent<Collider>().enabled = true;
                }
            }
            else
            {
                m_dropzone.Release();
            }
        }
        else if(m_icoPlanet)
        {       
            
            MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();
            interactable.isDistanceGrabbable = true;
            interactable.isGrabbable = true;

            m_icoPlanet.GetComponent<Collider>().enabled = true;
            foreach (IcoSegment segment in m_icoPlanet.Segments)
            {
                segment.GetComponent<MTK_Interactable>().isGrabbable = false;
                segment.GetComponent<Collider>().enabled = false;
            }
            m_icoPlanet.GenerateMeshCollider();

            m_icoPlanet = null;
        }
    }
}
