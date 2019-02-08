using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTerraformer : MonoBehaviour
{
    MTK_InputManager m_inputManager;
    Colonisator m_colonisator;
    
    void Awake ()
    {
        m_inputManager = GetComponentInParent<MTK_InputManager>();
        m_inputManager.m_onTrigger.AddListener(Terraform);
        m_inputManager.m_onGrip.AddListener(TerraformDown );
        m_inputManager.m_onPad.AddListener(Teleport);

        m_colonisator = FindObjectOfType<Colonisator>();
    }

    void Teleport(bool state)
    {
        if (state)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
            {
                IcoSegment segment = hit.collider.gameObject.GetComponent<IcoSegment>();
                if (segment)
                {
                    int index = 0;
                    foreach(IcoSegment seg in segment.icoPlanet.Segments)
                    {
                        if(seg == segment)
                        {
                            m_colonisator.selectedSegment = index;
                            break;
                        }
                        ++index;
                    }
                    

                }
            }
        }

    }

    void Terraform( bool state )
    {
        if( state)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
            {
                IcoSegment segment = hit.collider.gameObject.GetComponent<IcoSegment>();
                if (segment)
                {
                    ++segment.heightLevel;
                    segment.UpdateSegment();
                    segment.UpdateNeighbours();
                }
            }
        }       
    }

    void TerraformDown(bool state)
    {
        if (state)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f, LayerMask.GetMask("Planet")))
            {
                IcoSegment segment = hit.collider.gameObject.GetComponent<IcoSegment>();
                if (segment)
                {
                    --segment.heightLevel;
                    segment.UpdateSegment();
                    segment.UpdateNeighbours();
                }
            }
        }
    }

}
