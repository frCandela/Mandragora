using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcoPlanet : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool m_initialize = true;
    [SerializeField, Range(0, 3)] private int m_nbSubdivisions = 0;
    [SerializeField] private Material m_segmentMaterial;
    [SerializeField] public float heightDelta = 0.2f;
    [SerializeField] public float borderRatio = 0.2f;
    [SerializeField] public int nbLevels = 5;
    [SerializeField] public Color[] levelColors = new Color[10];
    [SerializeField] public bool updatePlanet = false;

    [Header("Read only data")]
    [SerializeField]  private int trianglesCount;

    [SerializeField, HideInInspector] private List<Vector3> m_vertices;
    [SerializeField, HideInInspector] private List<IcoSegment> m_segments;
    public List<IcoSegment> Segments { get { return m_segments; } }

    // Use this for initialization
    void Start()
    {
        m_vertices = Icosphere.GenerateIcosphere(1f, m_nbSubdivisions);

        if(m_initialize)
        {
            m_segments = new List<IcoSegment>();

            for (int i = 0; i < m_vertices.Count / 3; ++i)
            {
                Vector3 v0 = m_vertices[3 * i + 0];
                Vector3 v1 = m_vertices[3 * i + 1];
                Vector3 v2 = m_vertices[3 * i + 2];

                GameObject segment = new GameObject();
                segment.name = "segment" + i;
                segment.transform.parent = transform;
                segment.AddComponent<MeshRenderer>();
                segment.GetComponent<MeshRenderer>().material = m_segmentMaterial;
                segment.layer = LayerMask.NameToLayer("Planet");

                IcoSegment icoSeg = segment.AddComponent<IcoSegment>();
                m_segments.Add(icoSeg);
                icoSeg.heightLevel = 1;
                icoSeg.icoPlanet = this;
                icoSeg.SetBaseVertices(v0, v1, v2);
                icoSeg.GenerateCollider();
            }

            foreach (IcoSegment segment in m_segments)
            {
                segment.FindNeighbours();
            }
        }        

        foreach (IcoSegment segment in m_segments)
        {
            segment.UpdateSegment();
        }

    }

    private void Update()
    {
        if(updatePlanet)
        {
            trianglesCount = 0;

            updatePlanet = false;
            foreach (IcoSegment icoSeg in m_segments)
            {
                trianglesCount += icoSeg.triangleCount;
                icoSeg.UpdateSegment();
            }
        }
    }


}
