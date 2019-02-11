using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectPlacer : MonoBehaviour
{
    [SerializeField] private float m_hologramDistance = 0.5f;

    [SerializeField] private DropZone m_dropZone;
    [SerializeField] private Material m_material;
    [SerializeField] private GameObject m_previewObject;

    private List<MTK_InteractHand> m_handsInTrigger;

    private Dictionary<MTK_InteractHand, Hologram> m_objectsHolograms;
    private bool m_placingEnabled = false;

    Vector3 previewInitPos;
    Quaternion previewInitRos;

	// Use this for initialization
	void Start ()
    {
        m_objectsHolograms = new Dictionary<MTK_InteractHand, Hologram>();
        m_handsInTrigger = new List<MTK_InteractHand>();
        m_dropZone.onObjectCatched.AddListener(EnablePlacing);

        previewInitPos = m_previewObject.transform.position;
        previewInitRos = m_previewObject.transform.rotation;
    }

    void EnablePlacing( bool state )
    {
        IcoPlanet ico = m_dropZone.catchedObject.GetComponent<IcoPlanet>();

        if( state )
        {
            if(ico)
            {
                StartCoroutine(Animate()); //place after init ?

                ico.GetComponent<MeshCollider>().enabled = false;

                foreach (IcoSegment segment in ico.Segments)
                {
                    segment.GetComponent<MeshCollider>().enabled = true;
                    if (segment.GetComponent<IcoSegment>())
                    {
                        foreach (Transform decoration in segment.transform)
                        {
                            decoration.GetComponent<MTK_Interactable>().isGrabbable = true;
                        }
                    }
                }
            }
            else
            {
                m_dropZone.Release();
            }
        }
        else
        {
            m_placingEnabled = false;

            if (ico)
            {
                ico.GetComponent<MeshCollider>().enabled = true;
                foreach (Transform segment in m_dropZone.catchedObject.transform)
                {
                    if (segment.GetComponent<IcoSegment>())
                    {
                        segment.GetComponent<MeshCollider>().enabled = true;
                        foreach (Transform decoration in segment)
                        {
                            decoration.GetComponent<MTK_Interactable>().isGrabbable = false;
                        }
                    }
                }
            }
        }
    }

    IEnumerator Animate()
    {
        m_previewObject.SetActive(true);

        Hologram holo = CreateHologram(m_previewObject);
        Vector3 rotationAxis = (m_dropZone.transform.position - Camera.main.transform.position).normalized;

        for (float t = 0; t < 2; t += Time.deltaTime)
        {
            m_previewObject.transform.RotateAround(m_dropZone.transform.position, -rotationAxis, Time.deltaTime * 20);
            DrawHologram(holo, m_previewObject.transform);
            yield return new WaitForEndOfFrame();
        }

        Destroy(holo.gameObject);

        m_previewObject.SetActive(false);
        m_previewObject.transform.position = previewInitPos;
        m_previewObject.transform.rotation = previewInitRos;

        m_placingEnabled = true;

        m_dropZone.EnableButton();
    }

    Hologram CreateHologram(GameObject reference)
    {
        Hologram holo;

        GameObject go = new GameObject("hologram");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        holo = go.AddComponent<Hologram>();

        mf.mesh = reference.GetComponent<MeshFilter>().mesh;
        Material[] ghostMaterials = new Material[mf.mesh.subMeshCount];
        for (int i = 0; i < ghostMaterials.Length; i++)
            ghostMaterials[i] = m_material;
            
        mr.materials = ghostMaterials;
        go.transform.localScale = reference.transform.lossyScale;

        holo.referenceObject = reference;

        return holo;
    }

    void DrawHologram(Hologram holo, Transform originTransform)
    {
        Vector3 origin = originTransform.position;
        Vector3 dir = m_dropZone.transform.position - origin;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 1f, LayerMask.GetMask("Planet")))
        {
            Debug.DrawLine(origin, origin + dir, Color.green);

            if (hit.distance < m_hologramDistance)
            {
                holo.gameObject.SetActive(true);
                holo.gameObject.transform.position = hit.point;
                // holo.gameObject.transform.up = hit.normal;
                holo.gameObject.transform.rotation = originTransform.rotation;

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

    private void Update()
    {
        if(m_placingEnabled && m_dropZone.catchedObject.GetComponent<IcoPlanet>())
        {
            foreach (MTK_InteractHand hand in m_handsInTrigger)
            {
                if (hand.m_grabbed && hand.m_grabbed != m_dropZone.catchedObject && !hand.m_grabbed.GetComponent<IcoPlanet>())
                {
                    Hologram holo;
                    if (!m_objectsHolograms.TryGetValue(hand, out holo))
                    {
                        holo = CreateHologram(hand.m_grabbed.gameObject);
                        m_objectsHolograms.Add(hand, holo);
                    }

                    DrawHologram(holo, hand.m_grabbed.transform);
                }
                else if (m_objectsHolograms.ContainsKey(hand))
                {
                    Hologram holo = m_objectsHolograms[hand];

                    if(holo && holo.isActiveAndEnabled)
                    {
                        Outline outline = holo.referenceObject.GetComponent<Outline>();
                        if (outline)
                        {
                            outline.enabled = false;
                        }

                        GameObject copy = Instantiate(holo.referenceObject);

                        copy.transform.position = holo.transform.position;
                        copy.transform.rotation = holo.transform.rotation;
                        copy.transform.parent = holo.projectedSegment.transform;
                        copy.layer = LayerMask.NameToLayer("Planet");

                        MTK_Interactable interact = copy.GetComponent<MTK_Interactable>();
                        interact.isDroppable = false;
                        interact.isDistanceGrabbable = false;

                        copy.AddComponent<ObjectOnPlanet>().referenceObject = holo.referenceObject;

                        Destroy(copy.GetComponent<Rigidbody>());

                        holo.referenceObject.transform.position = -1000f * Vector3.up;
                        holo.referenceObject.GetComponent<Rigidbody>().isKinematic = true;

                        Destroy(holo.gameObject);
                        m_objectsHolograms.Remove(hand);

                        hand.GetComponentInParent<MTK_InputManager>().Haptic(1);
                        AkSoundEngine.PostEvent("Play_Pose", gameObject);
                    }
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
