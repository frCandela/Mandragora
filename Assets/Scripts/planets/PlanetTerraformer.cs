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
	
	// Update is called once per frame
    void EnableTerraformation( bool state )
    {
        if( state )
        {
            m_icoPlanet = m_dropzone.catchedObject.GetComponent<IcoPlanet>();
            if (m_icoPlanet)
            {
                MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();
                interactable.isDistanceGrabbable = false;
                interactable.isGrabbable = false;

                foreach( IcoSegment segment in m_icoPlanet.Segments)
                {
                    segment.GetComponent<MTK_Interactable>().isGrabbable = true;
                }
            }
        }
        else if(m_icoPlanet)
        {       
            
            MTK_Interactable interactable = m_icoPlanet.GetComponent<MTK_Interactable>();

            interactable.isDistanceGrabbable = true;
            interactable.isGrabbable = true;
            foreach (IcoSegment segment in m_icoPlanet.Segments)
            {
                segment.GetComponent<MTK_Interactable>().isGrabbable = false;
            }

            m_icoPlanet = null;
        }
    }
}
