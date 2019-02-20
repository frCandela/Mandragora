using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectPlacer : Workshop
{
    [SerializeField] private float m_hologramDistance = 0.5f;

    [SerializeField] private Material m_material;
    [SerializeField] private GameObject m_previewObject;

    private List<Pair<MTK_InteractHand, Hologram>> m_handsInTrigger;

    private bool m_placingEnabled = false;

    Vector3 previewInitPos;
    Quaternion previewInitRos;

    // Use this for initialization
    void Start ()
    {
        m_handsInTrigger = new List<Pair<MTK_InteractHand, Hologram>>();

        previewInitPos = m_previewObject.transform.position;
        previewInitRos = m_previewObject.transform.rotation;
    }

    protected override void OnWorkshopUpdateState(bool state, MTK_Interactable current)
    {
        IcoPlanet ico = m_dropzone.catchedObject.GetComponent<IcoPlanet>();

        if( state )
        {
            if(ico)
            {
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
                m_dropzone.Release();
            }
        }
        else
        {
            m_placingEnabled = false;

            if (ico)
            {
                ico.GetComponent<MeshCollider>().enabled = true;
                foreach (Transform segment in m_dropzone.catchedObject.transform)
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

    protected override IEnumerator AnimateWorkshop(float duration, VoidDelegate onFinish)
    {
        m_previewObject.SetActive(true);

        Hologram holo = CreateHologram(m_previewObject);
        Vector3 rotationAxis = (m_dropzone.transform.position - Camera.main.transform.position).normalized;

        for (float t = 0; t < 2; t += Time.deltaTime / duration)
        {
            // o - 1 - 1
            m_previewObject.transform.localScale = holo.transform.localScale = Vector3.one * 40 * Mathf.Lerp(0, 1, Mathf.PingPong(t, 1));

            m_previewObject.transform.RotateAround(m_dropzone.transform.position, -rotationAxis, Time.deltaTime * 20);
            DrawHologram(holo, m_previewObject.transform);
            yield return new WaitForEndOfFrame();
        }

        Destroy(holo.gameObject);

        m_previewObject.SetActive(false);
        m_previewObject.transform.position = previewInitPos;
        m_previewObject.transform.rotation = previewInitRos;

        m_placingEnabled = true;
        
        onFinish.Invoke();
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
        Vector3 dir = m_dropzone.transform.position - origin;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 1f, LayerMask.GetMask("Planet")))
        {
            if (hit.distance < m_hologramDistance)
            {
                holo.gameObject.SetActive(true);
                holo.gameObject.transform.position = hit.point;
                holo.gameObject.transform.rotation = originTransform.rotation;
                holo.projectedSegment = hit.collider.gameObject;
            }
            else
            {
                holo.gameObject.SetActive(false);
            }
        }
    }

    void AnchorHologram(Hologram holo)
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
    }

    private void Update()
    {
        Destroy(null);

        if( m_placingEnabled )
        {
            for (int i = 0; i < m_handsInTrigger.Count; ++i)
            {
                MTK_InteractHand hand = m_handsInTrigger[i].First;
                Hologram holo = m_handsInTrigger[i].Second;

                if (hand.m_grabbed != null )
                {
                    if(hand.m_grabbed != m_dropzone.catchedObject)
                    {
                        if( ! holo )
                        {
                            holo = CreateHologram(hand.m_grabbed.gameObject);
                            m_handsInTrigger[i].Second = holo;
                        }
                        holo.gameObject.SetActive(true);
                        DrawHologram(holo, hand.m_grabbed.transform);
                    }
                }
                else
                {
                    if( holo )
                    {
                        if (holo.isActiveAndEnabled)
                        {
                            AnchorHologram(holo);
                        }

                        hand.GetComponentInParent<MTK_InputManager>().Haptic(1);
                        AkSoundEngine.PostEvent("Play_Pose", gameObject);

                        m_handsInTrigger[i].Second = null;
                        Destroy(holo.gameObject);
                    }
                }
            }
        }
    }    

    void RegisterHand(MTK_InteractHand hand,  bool state)
    {
        if(state)
        {
            m_handsInTrigger.Add(new Pair<MTK_InteractHand, Hologram>( hand, null));
        }
        else
        {
            for( int i = 0; i < m_handsInTrigger.Count; ++i)
            {
                if(m_handsInTrigger[i].First == hand)
                {
                    if (m_handsInTrigger[i].Second)
                    {
                        Destroy(m_handsInTrigger[i].Second.gameObject);
                    }
                }
                m_handsInTrigger.RemoveAt(i);
                break;
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
                RegisterHand(hand, true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody)
        {
            MTK_InteractHand hand = other.attachedRigidbody.GetComponent<MTK_InteractHand>();
            if (hand)
            {
                RegisterHand(hand, false);
            }
        }
    }

    protected override void OnObjectGrabStart()
    {
        // throw new System.NotImplementedException();
    }

    protected override void OnObjectGrabStay()
    {
        // throw new System.NotImplementedException();
    }

    protected override void OnObjectGrabStop()
    {
        // throw new System.NotImplementedException();
    }
}
