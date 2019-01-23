using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectPlacer : MonoBehaviour
{
    [SerializeField] private float m_hologramDistance = 0.5f;

    [SerializeField] private DropZone m_dropZone;
    [SerializeField] private Material m_material;

    private List<MTK_InteractHand> m_handsInTrigger;

    private Dictionary<MTK_InteractHand, Hologram> m_objectsHolograms;
    private bool m_placingEnabled = false;

	// Use this for initialization
	void Start ()
    {
        m_objectsHolograms = new Dictionary<MTK_InteractHand, Hologram>();
        m_handsInTrigger = new List<MTK_InteractHand>();
        m_dropZone.onObjectCatched.AddListener(EnablePlacing);
    }

    void EnablePlacing( bool state )
    {
        if( state )
        {
            m_placingEnabled = true;
        }
        else
        {
            m_placingEnabled = false;
        }
    }

    private void Update()
    {
        if(m_placingEnabled && m_dropZone.catchedObject.GetComponent<IcoPlanet>())
        {
            foreach (MTK_InteractHand hand in m_handsInTrigger)
            {
                if (hand.m_grabbed && hand.m_grabbed != m_dropZone.catchedObject)
                {
                    Hologram holo;
                    if (!m_objectsHolograms.TryGetValue(hand, out holo))
                    {
                        GameObject go = new GameObject("hologram");
                        MeshFilter mf = go.AddComponent<MeshFilter>();
                        MeshRenderer mr = go.AddComponent<MeshRenderer>();
                        holo = go.AddComponent<Hologram>();

                        mf.mesh = hand.m_grabbed.GetComponent<MeshFilter>().mesh;
                        mr.material = m_material;
                        go.transform.localScale = hand.m_grabbed.transform.localScale;

                        holo.referenceObject = hand.m_grabbed.gameObject;

                        m_objectsHolograms.Add(hand, holo);
                    }                    

                    Vector3 origin = hand.m_grabbed.transform.position;
                    Vector3 dir = m_dropZone.transform.position - hand.m_grabbed.transform.position;


                    RaycastHit hit;
                    if (Physics.Raycast(origin, dir, out hit, 1f, LayerMask.GetMask("Planet")))
                    {
                        Debug.DrawLine(origin, origin + dir, Color.green);

                        if (hit.distance < m_hologramDistance)
                        {
                            holo.gameObject.SetActive(true);
                            holo.gameObject.transform.position = hit.point;
                            // holo.gameObject.transform.up = hit.normal;
                            holo.gameObject.transform.rotation = hand.m_grabbed.transform.rotation;

                            holo.projectedSegment = hit.collider.gameObject;
                        }
                        else
                        {
                            holo.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.DrawLine(origin, origin + dir, Color.red);
                    }
                }
                else if (m_objectsHolograms.ContainsKey(hand))
                {
                    Hologram holo = m_objectsHolograms[hand];

                    Outline outline = holo.referenceObject.GetComponent<Outline>();
                    if( outline)
                    {
                        outline.enabled = false;
                    }
                    


                    GameObject copy = Instantiate(holo.referenceObject);

                    copy.transform.position = holo.transform.position;
                    copy.transform.rotation = holo.transform.rotation;
                    copy.transform.parent = holo.projectedSegment.transform;
                    copy.layer = LayerMask.NameToLayer("Planet");

                    Destroy(copy.GetComponent<MTK_Interactable>());
                    Destroy(copy.GetComponent<MTK_JointType>());
                    Destroy(copy.GetComponent<Rigidbody>());
                    //Destroy(copy.GetComponent<Outline>());

                    holo.referenceObject.transform.position = -1000 * Vector3.up;


                    Destroy(holo.gameObject);
                    m_objectsHolograms.Remove(hand);
                }
            }
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if( other.attachedRigidbody)
        {
            MTK_InteractHand hand = other.attachedRigidbody.GetComponent<MTK_InteractHand>();
            
            if( hand)
            {
                m_handsInTrigger.Add(hand);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MTK_InteractHand hand = other.attachedRigidbody.GetComponent<MTK_InteractHand>();
        if (hand)
        {
            if (m_objectsHolograms.ContainsKey(hand))
                Destroy(m_objectsHolograms[hand].gameObject);

            m_handsInTrigger.Remove(hand);
            m_objectsHolograms.Remove(hand);
        }

    }


}
