using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcoPlanet : MonoBehaviour
{
    public bool Joined
    {
        set
        {
            if(value)
                AkSoundEngine.PostEvent("Soundseed_Play", gameObject);
            else
                AkSoundEngine.PostEvent("Soundseed_Stop", gameObject);
        }
    }

    [Header("Parameters")]
    [SerializeField] private bool m_initialize = true;
    [SerializeField, Range(0, 3)] public int m_nbSubdivisions = 0;
    [SerializeField] private Material m_segmentMaterial;

    [SerializeField] public float heightDelta = 0.2f;
    [SerializeField] public float borderRatio = 0.2f;
    [SerializeField] public int nbLevels = 5;
    [SerializeField] public int m_defaultHeightLevel = 0;

    [Header("Read only data")]
    [SerializeField]  private int trianglesCount;

    [SerializeField, HideInInspector] private List<Vector3> m_vertices;
    [SerializeField, HideInInspector] private List<IcoSegment> m_segments;
    public List<IcoSegment> Segments { get { return m_segments; } }


    Mesh m_mesh;
    MeshCollider m_meshCollider;

    List<MTK_PlanetSegmentJoint> m_segmentJoints = new List<MTK_PlanetSegmentJoint>();


    void Start()
    {
        Vector3 m_originalScale = transform.localScale;
        transform.localScale = Vector3.one * (transform.localScale.x /transform.lossyScale.x);

        if (m_initialize)
            Initialize();

        foreach (IcoSegment segment in m_segments)
            segment.GenerateCollider();

        foreach (IcoSegment segment in m_segments)
            segment.FindNeighbours();

        foreach (IcoSegment segment in m_segments)
            segment.UpdateSegment();

        transform.localScale = m_originalScale;


        m_mesh = new Mesh();
        m_meshCollider = GetComponent<MeshCollider>();
        if( ! m_meshCollider)
        {
            m_meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        m_meshCollider.convex = true;
        m_meshCollider.sharedMesh = m_mesh;

        GenerateMeshCollider();
        foreach (IcoSegment segment in m_segments)
            segment.GetComponent<MeshCollider>().enabled = false;
    }
    
    [ContextMenu("Animate")]
    public void Animate()
    {
        foreach (MTK_PlanetSegmentJoint seg in m_segmentJoints)
        {
            if(Random.Range(0,1.0f) < .5f)
                seg.RandomAnimation();
        }
    }

    [ContextMenu("GenerateMeshCollider")]
    public void GenerateMeshCollider()
    {
        List<Vector3> m_vertices = new List<Vector3>();
        List<int> m_indices = new List<int>();

        foreach (IcoSegment segment in m_segments)
        {
            int offset = m_vertices.Count;
            m_vertices.AddRange(segment.mesh.vertices);
            foreach (int index in segment.mesh.triangles)
            {
                m_indices.Add(index + offset);
            }
        }

        m_mesh.Clear();
        m_mesh.vertices = m_vertices.ToArray();
        m_mesh.triangles = m_indices.ToArray();
        
        m_meshCollider.sharedMesh = m_mesh;
    }

    // Use this for initialization
    void Initialize()
    {
        m_vertices = Icosphere.GenerateIcosphere(1f, m_nbSubdivisions);

        m_segments = new List<IcoSegment>();

        for (int i = 0; i < m_vertices.Count / 3; ++i)
        {
            Vector3 v0 = m_vertices[3 * i + 0];
            Vector3 v1 = m_vertices[3 * i + 1];
            Vector3 v2 = m_vertices[3 * i + 2];

            GameObject segment = new GameObject();
            segment.transform.position = transform.position;
            segment.name = "segment" + i;
            segment.transform.parent = transform;
            segment.AddComponent<MeshRenderer>();
            segment.GetComponent<MeshRenderer>().material = m_segmentMaterial;
            segment.layer = LayerMask.NameToLayer("Planet");

            IcoSegment icoSeg = segment.AddComponent<IcoSegment>();
            m_segments.Add(icoSeg);
            icoSeg.heightLevel = m_defaultHeightLevel;
            icoSeg.icoPlanet = this;
            icoSeg.SetBaseVertices(v0, v1, v2);

            m_segmentJoints.Add(segment.AddComponent<MTK_PlanetSegmentJoint>());
            MTK_Interactable interactable = segment.AddComponent<MTK_Interactable>();
            interactable.isDistanceGrabbable = false;
            interactable.isGrabbable = false;
            interactable.isDroppable = false;
        }
    }

    [ContextMenu("TestTesselation")]
    void TestTesselation()
    {
        SetTesselationLevel(3, 4);
    }

    public void SetTesselationLevel( int nbSubdivisions, int heightMultiplier = 1)
    {
        m_nbSubdivisions = nbSubdivisions;

        List<IcoSegment> oldSegmentsList = m_segments;

        // Generate new geometry
        Initialize();

        // Set height levels on the new geometry
        foreach (IcoSegment segment in m_segments)        
        {
            Vector3 center = segment.Center();
            float closestAngle = float.MaxValue;
            IcoSegment closestSegment = oldSegmentsList[0];

            foreach (IcoSegment oldSeg in oldSegmentsList)
            {
                float angle = Vector3.Angle(center, oldSeg.Center());
                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    closestSegment = oldSeg;
                }
            }
            segment.heightLevel = heightMultiplier * closestSegment.heightLevel;
        }

        foreach (IcoSegment segment in m_segments)
            segment.GenerateCollider();

        foreach (IcoSegment segment in m_segments)
            segment.FindNeighbours();

        foreach (IcoSegment segment in m_segments)
            segment.UpdateSegment();

        foreach (IcoSegment segment in oldSegmentsList)
            Destroy(segment);
    }

    [ContextMenu("UpdatePlanet")]
    public void UpdatePlanet()
    {
        trianglesCount = 0;
        foreach (IcoSegment icoSeg in m_segments)
        {
            trianglesCount += icoSeg.triangleCount;
            icoSeg.UpdateSegment();
        }
    }

    Quaternion m_oldRotation;
    private void Update()
    {
        AkSoundEngine.SetRTPCValue("Wind", Quaternion.Angle(transform.rotation, m_oldRotation) * 50);
        m_oldRotation = transform.rotation;
    }
}
