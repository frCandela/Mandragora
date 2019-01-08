using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class IcoSegment : MonoBehaviour
{
    [Range(-1, 5)] public int heightLevel = 0;

    public IcoSegment[] neighbours { get { return m_neighbours; } private set { } }


    //public IcoPlanet icoPlanet { private get; set; }
    public IcoPlanet icoPlanet;

    public int triangleCount { get { return m_triangles.Count/3; } }

    [SerializeField] private Vector3[] m_baseVertices = new Vector3[3];
    [SerializeField] private IcoSegment[] m_neighbours = new IcoSegment[3];

    private Mesh m_mesh;
    private Mesh m_sharedMeshCollider;
    private MeshCollider m_meshCollider;

    private List<Vector3> m_vertices = new List<Vector3>();
    private List<int> m_triangles = new List<int>();

    public enum SegmentType { full, corner1, corner2, corner3, side1, side2, side3, sidecorner, side2Corner, side3corner3, side3corner2 }
    private float m_lastUpdateTime = -1f;

    private void Awake()
    {
        m_mesh = new Mesh();
        m_mesh.name = name + "_mesh";
        GetComponent<MeshFilter>().mesh = m_mesh;

        m_sharedMeshCollider = new Mesh();
        m_meshCollider = GetComponent<MeshCollider>();
        m_meshCollider.convex = true;
    }

    [Range(0, 2)] public float testos = 0;
    public bool test = false;
    private void Update()
    {
        if (test)
        {
            test = false;
            //UpdateSegment();
        }

       /* if (heightLevel == 2)
        {
            Vector3 v0 = (1f + heightLevel * icoPlanet.heightDelta) * m_baseVertices[0];
            Debug.DrawLine(Center(), v0, Color.red);
            m_neighbours[0].GetComponent<MeshRenderer>().material.color = Color.red;

            int index = m_neighbours[0].IndexCorrespondingleftVertice(this);
            Vector3 v0b = (1f + heightLevel * icoPlanet.heightDelta) * m_neighbours[0].m_baseVertices[index];
            Debug.DrawLine(m_neighbours[0].Center(), v0b, Color.red);
        }*/
    }

    private Vector3 Center()
    {
        Vector3 v0 = (1f + heightLevel * icoPlanet.heightDelta) * m_baseVertices[0];
        Vector3 v1 = (1f + heightLevel * icoPlanet.heightDelta) * m_baseVertices[1];
        Vector3 v2 = (1f + heightLevel * icoPlanet.heightDelta) * m_baseVertices[2];
        
        return (v0 + v1 + v2) / 3f;
    }

    public IcoSegment LeftFrom( IcoSegment segment )
    {
        for( int i = 0; i < 3; ++i)
        {
            if( m_neighbours[i] == segment )
            {
                return m_neighbours[(i + 1) % 3];
            }
        }
        return null;
    }

    public void TestMethod()
    {
        GenerateBaseGeometry(ref m_vertices, ref m_triangles);
        SetCollider(m_vertices, m_triangles);
        MakeSide3Corner3();
        BakeNormals();
    }

    public void TestMethod2()// thumb button
    {
        GenerateBaseGeometry(ref m_vertices, ref m_triangles);
        SetCollider(m_vertices, m_triangles);
        MakeSide3Corner3();
        BakeNormals();
    }

    public void UpdateSegment(bool canUpdateNeighbours = false)
    {
        if(m_lastUpdateTime != Time.time)
        {
            m_lastUpdateTime = Time.time;

            heightLevel = Mathf.Clamp(heightLevel, 0, icoPlanet.nbLevels);
            Color color = icoPlanet.levelColors[Mathf.Clamp(heightLevel, 0, icoPlanet.levelColors.Length - 1)];
            GetComponent<MeshRenderer>().material.color = color;

            GenerateBaseGeometry(ref m_vertices, ref m_triangles);
            //SetCollider(m_vertices, m_triangles);

            SegmentType type;
            int orientation;
            SegmentTypeSelector.FindSegmentType(this, out type, out orientation);

            switch (type)
            {
                case SegmentType.corner1:
                    MakeCorner(orientation);
                    break;
                case SegmentType.corner2:
                    MakeCorner2(orientation);
                    break;
                case SegmentType.corner3:
                    MakeCorner3();
                    break;
                case SegmentType.side1:
                    MakeSide(orientation);
                    break;
                case SegmentType.side2:
                    MakeSide2(orientation);
                    break;
                case SegmentType.side3:
                    MakeSide3();
                    break;
                case SegmentType.sidecorner:
                    MakeCornerSide(orientation);
                    break;
                case SegmentType.side3corner3:
                    MakeSide3Corner3();
                    break;
                case SegmentType.side2Corner:
                    MakeSide2Corner(orientation);
                    break;
                case SegmentType.side3corner2:
                    MakeSide3Corner2(orientation);
                    break;                    
                case SegmentType.full:
                    break;
                default:
                    print("error");
                    break;
            }            
            BakeNormals();

            SetCollider(m_vertices, m_triangles);
        }       
    }

    public void FindNeighbours()
    {
        Vector3 center = 0.9f * (m_baseVertices[0] + m_baseVertices[1] + m_baseVertices[2]) / 3f;
        for (int i = 0; i < 3; ++i)
        {
            Vector3 edge = 0.9f * (m_baseVertices[i] + m_baseVertices[(i + 1) % 3]) / 2;
            Ray ray = new Ray(center, edge - center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("Planet")))
            {
                m_neighbours[i] = hit.collider.GetComponent<IcoSegment>();
            }
        }
    }

    public void SetBaseVertices(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        m_baseVertices[0] = v0;
        m_baseVertices[1] = v1;
        m_baseVertices[2] = v2;
    }

    public void UpdateNeighbours()
    {
        for (int i = 0; i < 3; ++i)
        {
            UpdateFullLoopLeft(this, i);
        }
    }

    static bool UpdateFullLoopLeft(IcoSegment segment, int side)
    {
        IcoSegment current = segment.neighbours[side];
        IcoSegment previous = segment;
        while (current != segment)
        {
            current.UpdateSegment();
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return true;
    }

    public void GenerateCollider()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        GenerateBaseGeometry(ref vertices, ref triangles);
        SetCollider(vertices, triangles);
    }


    // Generation

    private void SetCollider(List<Vector3> vertices, List<int> triangles)
    {
        // Set collider
        m_sharedMeshCollider.Clear();
        m_sharedMeshCollider.vertices = vertices.ToArray();
        m_sharedMeshCollider.triangles = triangles.ToArray();
        m_meshCollider.sharedMesh = m_sharedMeshCollider;
    }

    private void BakeNormals()
    {
        // Update geometry 
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < m_triangles.Count; ++i)
        {
            newVertices.Add(m_vertices[m_triangles[i]]);
            newTriangles.Add(i);
        }
        m_mesh.Clear();
        m_mesh.vertices = newVertices.ToArray();
        m_mesh.triangles = newTriangles.ToArray();
        m_mesh.RecalculateNormals();
    }

    private void GenerateBaseGeometry(ref List<Vector3> vertices, ref List<int> triangles)
    {
        vertices.Clear();
        triangles.Clear();

        vertices.Add(Vector3.zero);
        vertices.Add(m_baseVertices[0] + heightLevel * icoPlanet.heightDelta * m_baseVertices[0]);
        vertices.Add(m_baseVertices[1] + heightLevel * icoPlanet.heightDelta * m_baseVertices[1]);
        vertices.Add(m_baseVertices[2] + heightLevel * icoPlanet.heightDelta * m_baseVertices[2]);

        triangles.Add(1); triangles.Add(2); triangles.Add(3);
        triangles.Add(1); triangles.Add(0); triangles.Add(2);
        triangles.Add(2); triangles.Add(0); triangles.Add(3);
        triangles.Add(3); triangles.Add(0); triangles.Add(1);
    }

    // Procedural generation 

    void MakeCorner(int corner)
    {
        SetElevationTriangle(0, heightLevel);
        CutCorner(0, corner, icoPlanet.borderRatio);
        SetElevation(corner, heightLevel - 1);
    }

    void MakeCorner2(int oppositeCorner)
    {
        CutTwoCorners(0, oppositeCorner, icoPlanet.borderRatio);
        SetElevationVertex((oppositeCorner + 1) % 3 + 1, heightLevel - 1);
        SetElevationVertex((oppositeCorner + 2) % 3 + 1, heightLevel - 1);
    }

    void MakeCorner3()
    {
        GenerateBaseGeometry(ref m_vertices, ref m_triangles);
        SetElevationTriangle(0, heightLevel);
        CutAllCorners(0, icoPlanet.borderRatio);
        SetElevationVertex(1, heightLevel - 1);
        SetElevationVertex(2, heightLevel - 1);
        SetElevationVertex(3, heightLevel - 1);
    }

    void MakeCornerSide(int cornerEdge)
    {
        SetElevationTriangle(0, heightLevel - 1);
        Extrude(0, 0f);
        SetElevationTriangle(0, heightLevel);
        MoveTowardsEdge((cornerEdge + 0) % 3, (cornerEdge + 2) % 3, icoPlanet.borderRatio);
        MoveTowardsEdge((cornerEdge + 1) % 3, (cornerEdge + 2) % 3, icoPlanet.borderRatio);
        CutCorner(0, (cornerEdge + 2) % 3, icoPlanet.borderRatio / (1f- icoPlanet.borderRatio));
        SetElevation((cornerEdge + 2) % 3, heightLevel - 1);
    }

    void MakeSide(int oppositeCorner)
    {
        SetElevationTriangle(0, heightLevel - 1);
        Extrude(0, 0);
        SetElevationTriangle(0, heightLevel);
        MoveTowardsEdge((oppositeCorner + 1) % 3, oppositeCorner, icoPlanet.borderRatio);
        MoveTowardsEdge((oppositeCorner + 2) % 3, oppositeCorner, icoPlanet.borderRatio);
    }

    void MakeSide2(int oppositeSide)
    {
        SetElevationTriangle(0, heightLevel - 1);
        Extrude(0, 0f);
        SetElevationTriangle(0, heightLevel);
        Scale(0, (1f-2* icoPlanet.borderRatio));
        ClampToEdge(0, oppositeSide, heightLevel);
    }

    void MakeSide3()
    {
        SetElevationTriangle(0, heightLevel - 1);
        Extrude(0, 0f);
        SetElevationTriangle(0, heightLevel);
        Scale(0, (1f - 2 * icoPlanet.borderRatio));
    }

    void MakeSide2Corner( int cornerIndex)
    {
        int max = Mathf.Max(m_neighbours[(cornerIndex ) % 3].heightLevel, m_neighbours[(cornerIndex + 2) % 3].heightLevel);
        if( max == heightLevel - 1)
        {
            ExtrudeWithCorner1(0, cornerIndex, icoPlanet.borderRatio, heightLevel - 1, max, false);
        }
        else
        {
            ExtrudeWithCorner1(0, cornerIndex, icoPlanet.borderRatio, heightLevel - 1, max );
        }
    }

    void MakeSide3Corner3()
    {
        int[] maxHeights = new int[] {
            Mathf.Max(m_neighbours[2].heightLevel, m_neighbours[0].heightLevel),
            Mathf.Max(m_neighbours[1].heightLevel, m_neighbours[0].heightLevel),
            Mathf.Max(m_neighbours[1].heightLevel, m_neighbours[2].heightLevel),
        };


        SetElevationTriangle(0, heightLevel - 1);
        ExtrudeWithCorners3(0, maxHeights, icoPlanet.borderRatio);
        SetElevationTriangle(0, heightLevel );

    }

    void MakeSide3Corner2(int noCornerIndex)
    {
        ExtrudeWithCorners2(0, noCornerIndex, icoPlanet.borderRatio, heightLevel - 1);
        SetElevationVertex((noCornerIndex + 0) % 3 + 1, heightLevel - 1);
        SetElevationVertex((noCornerIndex + 1) % 3 + 1, heightLevel - 2);
        SetElevationVertex((noCornerIndex + 2) % 3 + 1, heightLevel - 2);
    }

    // Mesh manipulation methods
    private void Extrude(int indiceIndexTriangle, float ratio)
    {
        Vector3 v0 = m_vertices[m_triangles[indiceIndexTriangle]];
        Vector3 v1 = m_vertices[m_triangles[indiceIndexTriangle + 1]];
        Vector3 v2 = m_vertices[m_triangles[indiceIndexTriangle + 2]];
        int v0i = m_triangles[indiceIndexTriangle];
        int v1i = m_triangles[indiceIndexTriangle + 1];
        int v2i = m_triangles[indiceIndexTriangle + 2];

        Vector3 center = (v0 + v1 + v2) / 3f;
        Vector3 centerDir = center.normalized;

        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v0 + ratio * (center - v0));//0
        m_vertices.Add(v1 + ratio * (center - v1));//1
        m_vertices.Add(v2 + ratio * (center - v2));//2

        // Extruded top triangle
        m_triangles[indiceIndexTriangle] = indexFirstVertice;
        m_triangles[indiceIndexTriangle + 1] = indexFirstVertice + 1;
        m_triangles[indiceIndexTriangle + 2] = indexFirstVertice + 2;

        // One edge
        m_triangles.Add(indexFirstVertice);
        m_triangles.Add(indexFirstVertice + 2);
        m_triangles.Add(v0i);

        m_triangles.Add(v2i);
        m_triangles.Add(v0i);
        m_triangles.Add(indexFirstVertice + 2);

        // Second edge
        m_triangles.Add(indexFirstVertice + 1);
        m_triangles.Add(indexFirstVertice + 0);
        m_triangles.Add(v1i);

        m_triangles.Add(v0i);
        m_triangles.Add(v1i);
        m_triangles.Add(indexFirstVertice + 0);

        // Third edge
        m_triangles.Add(indexFirstVertice + 2);
        m_triangles.Add(indexFirstVertice + 1);
        m_triangles.Add(v2i);

        m_triangles.Add(v1i);
        m_triangles.Add(v2i);
        m_triangles.Add(indexFirstVertice + 1);
    }

    private void ExtrudeWithCorners3(int indiceIndexTriangle, int[] cornersLevel, float ratio, bool fillGaps = true)
    {
        int i0 = m_triangles[indiceIndexTriangle + 0];
        int i1 = m_triangles[indiceIndexTriangle + 1];
        int i2 = m_triangles[indiceIndexTriangle + 2];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        Vector3 v01 = v0 + ratio * (v1 - v0);
        Vector3 v10 = v1 + ratio * (v0 - v1);
        Vector3 v12 = v1 + ratio * (v2 - v1);
        Vector3 v21 = v2 + ratio * (v1 - v2);
        Vector3 v20 = v2 + ratio * (v0 - v2);
        Vector3 v02 = v0 + ratio * (v2 - v0);
        Vector3 a = v01 + (v02 - v0);
        Vector3 b = v12 + (v10 - v1);
        Vector3 c = v20 + (v21 - v2);

        Vector3[] viNormed = new[] { v0.normalized, v1.normalized, v2.normalized};

        Vector3[] v0Bis = new[] {
            (1f + cornersLevel[0] * icoPlanet.heightDelta) * viNormed[0],
            (1f + cornersLevel[1] * icoPlanet.heightDelta) * viNormed[0],
            (1f + cornersLevel[2] * icoPlanet.heightDelta) * viNormed[0]
        };
        Vector3[] v1Bis = new[] {
            (1f + cornersLevel[0] * icoPlanet.heightDelta) * viNormed[1],
            (1f + cornersLevel[1] * icoPlanet.heightDelta) * viNormed[1],
            (1f + cornersLevel[2] * icoPlanet.heightDelta) * viNormed[1]
        };
        Vector3[] v2Bis = new[] {
            (1f + cornersLevel[0] * icoPlanet.heightDelta) * viNormed[2],
            (1f + cornersLevel[1] * icoPlanet.heightDelta) * viNormed[2],
            (1f + cornersLevel[2] * icoPlanet.heightDelta) * viNormed[2]
        };

        Vector3 v01Bis = v0Bis[0] + ratio * (v1Bis[0] - v0Bis[0]);
        Vector3 v02Bis = v0Bis[0] + ratio * (v2Bis[0] - v0Bis[0]);
        Vector3 v10Bis = v1Bis[1] + ratio * (v0Bis[1] - v1Bis[1]);
        Vector3 v12Bis = v1Bis[1] + ratio * (v2Bis[1] - v1Bis[1]);
        Vector3 v21Bis = v2Bis[2] + ratio * (v1Bis[2] - v2Bis[2]);
        Vector3 v20Bis = v2Bis[2] + ratio * (v0Bis[2] - v2Bis[2]);



        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v01);
        m_vertices.Add(v10);
        m_vertices.Add(v12);
        m_vertices.Add(v21);
        m_vertices.Add(v20);
        m_vertices.Add(v02);
        m_vertices.Add(a);
        m_vertices.Add(b);
        m_vertices.Add(c);
        m_vertices.Add(v01Bis);
        m_vertices.Add(v10Bis);
        m_vertices.Add(v12Bis);
        m_vertices.Add(v21Bis);
        m_vertices.Add(v20Bis);
        m_vertices.Add(v02Bis);
        
        for (int i = 0; i < 3; ++i)
        {
            if (SegmentTypeSelector.LoopLeftBelowLevel(this, (i)%3 , cornersLevel[i]))
            {
                m_vertices.Add((1f + cornersLevel[i] * icoPlanet.heightDelta) * viNormed[i]);
            }
            else
            {
                m_vertices.Add((1f + (cornersLevel[i]-1) * icoPlanet.heightDelta) * viNormed[i]);
            }
        }





        int i01 = indexFirstVertice + 0;
        int i10 = indexFirstVertice + 1;
        int i12 = indexFirstVertice + 2;
        int i21 = indexFirstVertice + 3;
        int i20 = indexFirstVertice + 4;
        int i02 = indexFirstVertice + 5;
        int ia = indexFirstVertice + 6;
        int ib = indexFirstVertice + 7;
        int ic = indexFirstVertice + 8;
        int i01Bis = indexFirstVertice + 9;
        int i10Bis = indexFirstVertice + 10;
        int i12Bis = indexFirstVertice + 11;
        int i21Bis = indexFirstVertice + 12;
        int i20Bis = indexFirstVertice + 13;
        int i02Bis = indexFirstVertice + 14;
        int i0Bis = indexFirstVertice + 15;
        int i1Bis = indexFirstVertice + 16;
        int i2Bis = indexFirstVertice + 17;

        int min = Mathf.Min(cornersLevel);
        SetElevationTriangle(indiceIndexTriangle, min - 1);




        m_triangles[indiceIndexTriangle + 0] = ia;
        m_triangles[indiceIndexTriangle + 1] = ib;
        m_triangles[indiceIndexTriangle + 2] = ic;

        //top side
        AddTriangle(i01, i10, ia);
        AddTriangle(ia, i10, ib);
        //top side
        AddTriangle(i12, i21, ib);
        AddTriangle(ib, i21, ic);
        //top side
        AddTriangle(i20, i02, ic);
        AddTriangle(ic, i02, ia);
        
        //Corner
        AddTriangle(i20Bis, i20, i21);
        AddTriangle(i20Bis, i21, i21Bis);
        AddTriangle(i2Bis, i20Bis, i21Bis);//
        AddTriangle(i20, ic, i21);
        //Corner
        AddTriangle(i01Bis, i01, i02);
        AddTriangle(i01Bis, i02, i02Bis);
        AddTriangle(i0Bis, i01Bis, i02Bis);//
        AddTriangle(i01, ia, i02);
        //Corner
        AddTriangle(i12Bis, i12, i10);
        AddTriangle(i12Bis, i10, i10Bis);
        AddTriangle(i1Bis, i12Bis, i10Bis);//
        AddTriangle(i12, ib, i10);
        
        // mid side
        AddTriangle(i02, i20, i20Bis);
        AddTriangle(i02, i20Bis, i02Bis);
        // mid side
        AddTriangle(i10, i01, i01Bis);
        AddTriangle(i10, i01Bis, i10Bis);
        // mid side
        AddTriangle(i21, i12, i12Bis);
        AddTriangle(i21, i12Bis, i21Bis);

        if ( fillGaps)
        {
            AddTriangle(i0, i0Bis, i02Bis);
            AddTriangle(i0, i02Bis, i20Bis);
            AddTriangle(i0, i20Bis, i2);
            AddTriangle(i2, i20Bis, i2Bis);

            AddTriangle(i1, i1Bis, i10Bis);
            AddTriangle(i1, i10Bis, i01Bis);
            AddTriangle(i1, i01Bis, i0);
            AddTriangle(i0, i01Bis, i0Bis);

            AddTriangle(i2, i2Bis, i21Bis);
            AddTriangle(i2, i21Bis, i12Bis);
            AddTriangle(i2, i12Bis, i1);
            AddTriangle(i1, i12Bis, i1Bis);
        }
    }

    private void ExtrudeWithCorners2(int indiceIndexTriangle, int noCornerIndex, float ratio, int cornersLevel, bool fillGaps = true)
    {
        int i0 = m_triangles[indiceIndexTriangle + (noCornerIndex + 1) % 3];
        int i1 = m_triangles[indiceIndexTriangle + (noCornerIndex + 2) % 3];
        int i2 = m_triangles[indiceIndexTriangle + (noCornerIndex + 3) % 3];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        Vector3 v01 = v0 + ratio * (v1 - v0);
        Vector3 v10 = v1 + ratio * (v0 - v1);
        Vector3 v12 = v1 + ratio * (v2 - v1);
        Vector3 v21 = v2 + ratio * (v1 - v2);
        Vector3 v20 = v2 + ratio * (v0 - v2);
        Vector3 v02 = v0 + ratio * (v2 - v0);
        Vector3 a = v01 + (v02 - v0);
        Vector3 b = v12 + (v10 - v1);
        Vector3 c = v20 + (v21 - v2);

        Vector3 tmpv0 = (1f + cornersLevel * icoPlanet.heightDelta) * m_vertices[i0].normalized;
        Vector3 tmpv1 = (1f + cornersLevel * icoPlanet.heightDelta) * m_vertices[i1].normalized;
        Vector3 tmpv2 = (1f + cornersLevel * icoPlanet.heightDelta) * m_vertices[i2].normalized;
        v01 = tmpv0 + ratio * (tmpv1 - tmpv0);
        v02 = tmpv0 + ratio * (tmpv2 - tmpv0);
        v10 = tmpv1 + ratio * (tmpv0 - tmpv1);
        v12 = tmpv1 + ratio * (tmpv2 - tmpv1);

        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v01);
        m_vertices.Add(v10);
        m_vertices.Add(v12);
        m_vertices.Add(v02);
        m_vertices.Add(a);
        m_vertices.Add(b);
        m_vertices.Add(c);

        int i01 = indexFirstVertice + 0;
        int i10 = indexFirstVertice + 1;
        int i12 = indexFirstVertice + 2;
        int i02 = indexFirstVertice + 3;
        int ia = indexFirstVertice + 4;
        int ib = indexFirstVertice + 5;
        int ic = indexFirstVertice + 6;

        m_triangles[indiceIndexTriangle + 0] = ia;
        m_triangles[indiceIndexTriangle + 1] = ib;
        m_triangles[indiceIndexTriangle + 2] = ic;

        AddTriangle(i0, i01, i02);
        AddTriangle(i01, ia, i02);

        AddTriangle(i10, i1, i12);
        AddTriangle(i10, i12, ib);

        AddTriangle(i02, ic, i2);
        AddTriangle(i02, ia, ic);

        AddTriangle(ib, i12, ic);
        AddTriangle(ic, i12, i2);

        AddTriangle(i01, i10, ia);
        AddTriangle(ia, i10, ib);

        if (fillGaps)
        {
            AddTriangle(i0, i02, i2);
            AddTriangle(i1, i2, i12);

            AddTriangle(i0, i1, i01);
            AddTriangle(i1, i10, i01);
        }
    }

    private void ExtrudeWithCorner1(int indiceIndexTriangle, int cornerIndex, float ratio, int cornerLevelUp, int cornerLevelDown,  bool fillGaps = true)
    {
        int i0 = m_triangles[indiceIndexTriangle + (cornerIndex + 0) % 3];
        int i1 = m_triangles[indiceIndexTriangle + (cornerIndex + 1) % 3];
        int i2 = m_triangles[indiceIndexTriangle + (cornerIndex + 2) % 3];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        Vector3 v01 = v0 + ratio * (v1 - v0);
        Vector3 v02 = v0 + ratio * (v2 - v0);

        Vector3 v12 = v1 + ratio * (v2 - v1);
        Vector3 v21 = v2 + ratio * (v1 - v2);

        Vector3 a = v01 + (v02 - v0);


        Vector3[] viNormed = new[] { v0.normalized, v1.normalized, v2.normalized };
        Vector3[] v0Bis = new[] {
            (1f + cornerLevelUp * icoPlanet.heightDelta) * viNormed[0],
            (1f + cornerLevelDown * icoPlanet.heightDelta) * viNormed[0],
        };
        Vector3[] v1Bis = new[] {
            (1f + cornerLevelUp * icoPlanet.heightDelta) * viNormed[1],
            (1f + cornerLevelDown * icoPlanet.heightDelta) * viNormed[1],
        };
        Vector3[] v2Bis = new[] {
            (1f + cornerLevelUp * icoPlanet.heightDelta) * viNormed[2],
            (1f + cornerLevelDown * icoPlanet.heightDelta) * viNormed[2],
        };


        v01 = v0Bis[0] + ratio * (v1Bis[0] - v0Bis[0]);
        v02 = v0Bis[0] + ratio * (v2Bis[0] - v0Bis[0]);
        Vector3 v01Bis = v0Bis[1] + ratio * (v1Bis[1] - v0Bis[1]);
        Vector3 v02Bis = v0Bis[1] + ratio * (v2Bis[1] - v0Bis[1]);


        if (SegmentTypeSelector.LoopLeftBelowLevel(this, cornerIndex, cornerLevelDown))
        {
                m_vertices[i0] = (1f + (cornerLevelDown) * icoPlanet.heightDelta) * viNormed[0];
        }
        else
        {
                m_vertices[i0] = (1f + (cornerLevelDown - 1) * icoPlanet.heightDelta) * viNormed[0];
        }  

        m_vertices[i1] = v1Bis[1];
        m_vertices[i2] = v2Bis[1];


        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v01);
        m_vertices.Add(v02);
        m_vertices.Add(v12);
        m_vertices.Add(v21);
        m_vertices.Add(a);
        m_vertices.Add(v01Bis);
        m_vertices.Add(v02Bis);
        m_vertices.Add(v0Bis[0]);
        m_vertices.Add(v1Bis[0]);
        m_vertices.Add(v2Bis[0]);

        int i01 = indexFirstVertice + 0;
        int i02 = indexFirstVertice + 1;
        int i12 = indexFirstVertice + 2;
        int i21 = indexFirstVertice + 3;
        int ia = indexFirstVertice + 4;
        int i01Bis = indexFirstVertice + 5;
        int i02Bis = indexFirstVertice + 6;
        int i0Bis = indexFirstVertice + 7;
        int i1Bis = indexFirstVertice + 8; 
        int i2Bis = indexFirstVertice + 9;

        m_triangles[indiceIndexTriangle + 0] = ia;
        m_triangles[indiceIndexTriangle + 1] = i12;
        m_triangles[indiceIndexTriangle + 2] = i21;

        //Top edges
        AddTriangle(i01, i1Bis, ia);
        AddTriangle(i1Bis, i12, ia);
        //Top edges
        AddTriangle(i02, ia, i21);
        AddTriangle(i21, i2Bis, i02);

        // corner triangles
        AddTriangle(i01, ia, i02);
        AddTriangle(i01Bis, i02Bis, i0);

        // Vertical sides
        AddTriangle(i0, i1, i01Bis);
        AddTriangle(i0, i02Bis, i2 );

        if (fillGaps)
        {        
            // front square
            AddTriangle(i01Bis, i01, i02);
            AddTriangle(i01Bis, i02, i02Bis);

            // Vertical sides
            AddTriangle( i1, i1Bis, i01);            
            AddTriangle(i1, i01, i01Bis);

            AddTriangle(i2Bis, i2, i02);
            AddTriangle(i02, i2, i02Bis);
        }
    }

    private void SetElevationTriangle(int indiceIndexTriangle, int level)
    {
        m_vertices[m_triangles[indiceIndexTriangle]] = (1f + level * icoPlanet.heightDelta) * m_vertices[m_triangles[indiceIndexTriangle]].normalized;
        m_vertices[m_triangles[indiceIndexTriangle + 1]] = (1f + level * icoPlanet.heightDelta) * m_vertices[m_triangles[indiceIndexTriangle + 1]].normalized;
        m_vertices[m_triangles[indiceIndexTriangle + 2]] = (1f + level * icoPlanet.heightDelta) * m_vertices[m_triangles[indiceIndexTriangle + 2]].normalized;
    }

    private void SetElevation(int indiceIndexVertex, int level)
    {
        m_vertices[m_triangles[indiceIndexVertex]] = (1f + level * icoPlanet.heightDelta) * m_vertices[m_triangles[indiceIndexVertex]].normalized;
    }

    private void SetElevationVertex(int indiceVertex, int level)
    {
        m_vertices[indiceVertex] = (1f + level * icoPlanet.heightDelta) * m_vertices[indiceVertex].normalized;
    }

    private void Scale(int indiceIndexTriangle, float scale)
    {
        Vector3 v0 = m_vertices[m_triangles[indiceIndexTriangle]];
        Vector3 v1 = m_vertices[m_triangles[indiceIndexTriangle + 1]];
        Vector3 v2 = m_vertices[m_triangles[indiceIndexTriangle + 2]];
        Vector3 center = (v0 + v1 + v2) / 3f;

        m_vertices[m_triangles[indiceIndexTriangle]] = center + scale * (v0 - center);
        m_vertices[m_triangles[indiceIndexTriangle + 1]] = center + scale * (v1 - center);
        m_vertices[m_triangles[indiceIndexTriangle + 2]] = center + scale * (v2 - center);
    }

    private void ClampToEdge(int indiceIndexTriangle, int edgeIndex, float level)
    {
        Vector3 v0 = m_vertices[m_triangles[indiceIndexTriangle + (edgeIndex) % 3]];
        Vector3 v1 = m_vertices[m_triangles[indiceIndexTriangle + (edgeIndex + 1) % 3]];
        Vector3 v01 = (v0 + v1) / 2f;

        float height = (1f + level * icoPlanet.heightDelta);
        Vector3 centerTargetEdge = height * (m_baseVertices[edgeIndex % 3] + m_baseVertices[(edgeIndex + 1) % 3]) / 2f;
        Vector3 dir = centerTargetEdge - v01;

        m_vertices[m_triangles[indiceIndexTriangle + (edgeIndex + 0) % 3]] += dir;
        m_vertices[m_triangles[indiceIndexTriangle + (edgeIndex + 1) % 3]] += dir;
        m_vertices[m_triangles[indiceIndexTriangle + (edgeIndex + 2) % 3]] += dir;
    }

    private void CutCorner(int indiceIndexTriangle, int cornerIndex, float ratio)
    {
        int i0 = m_triangles[indiceIndexTriangle + (cornerIndex) % 3];
        int i1 = m_triangles[indiceIndexTriangle + (cornerIndex + 1) % 3];
        int i2 = m_triangles[indiceIndexTriangle + (cornerIndex + 2) % 3];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v0 + ratio * (v1 - v0));//0
        m_vertices.Add(v0 + ratio * (v2 - v0));//1

        m_triangles[indiceIndexTriangle + (cornerIndex + 1) % 3] = indexFirstVertice + 0;
        m_triangles[indiceIndexTriangle + (cornerIndex + 2) % 3] = indexFirstVertice + 1;

        m_triangles.Add(i1);
        m_triangles.Add(i2);
        m_triangles.Add(indexFirstVertice + 0);

        m_triangles.Add(i2);
        m_triangles.Add(indexFirstVertice + 1);
        m_triangles.Add(indexFirstVertice + 0);

        m_triangles.Add(i0);
        m_triangles.Add(i1);
        m_triangles.Add(indexFirstVertice + 0);

        m_triangles.Add(i2);
        m_triangles.Add(i0);
        m_triangles.Add(indexFirstVertice + 1);

    }

    private void CutTwoCorners(int indiceIndexTriangle, int oppositeCorner, float ratio, bool fillGaps = true)
    {
        int i0 = m_triangles[indiceIndexTriangle + (oppositeCorner) % 3];
        int i1 = m_triangles[indiceIndexTriangle + (oppositeCorner + 1) % 3];
        int i2 = m_triangles[indiceIndexTriangle + (oppositeCorner + 2) % 3];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        Vector3 v10 = v1 + ratio * (v0 - v1);
        Vector3 v12 = v1 + ratio * (v2 - v1);
        Vector3 v21 = v2 + ratio * (v1 - v2);
        Vector3 v20 = v2 + ratio * (v0 - v2);

        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v10);
        m_vertices.Add(v12);
        m_vertices.Add(v21);
        m_vertices.Add(v20);

        int i10 = indexFirstVertice + 0;
        int i12 = indexFirstVertice + 1;
        int i21 = indexFirstVertice + 2;
        int i20 = indexFirstVertice + 3;

        m_triangles[indiceIndexTriangle] = i0;
        m_triangles[indiceIndexTriangle+1] = i10;
        m_triangles[indiceIndexTriangle+2] = i20;

        AddTriangle(i10, i1, i12);
        AddTriangle(i10, i12, i20);
        AddTriangle(i12, i21, i20);
        AddTriangle(i21, i2, i20);

        if (fillGaps)
        {
            AddTriangle(i0, i1, i10);

            AddTriangle(i0, i20, i2);

            AddTriangle(i1, i2, i12);
            AddTriangle(i12, i2, i21);


        }
    }

    private void CutAllCorners(int indiceIndexTriangle, float ratio, bool fillGaps = true)
    {
        int i0 = m_triangles[indiceIndexTriangle + 0];
        int i1 = m_triangles[indiceIndexTriangle + 1];
        int i2 = m_triangles[indiceIndexTriangle + 2];

        Vector3 v0 = m_vertices[i0];
        Vector3 v1 = m_vertices[i1];
        Vector3 v2 = m_vertices[i2];

        Vector3 v01 = v0 + ratio * (v1 - v0);
        Vector3 v10 = v1 + ratio * (v0 - v1);
        Vector3 v12 = v1 + ratio * (v2 - v1);
        Vector3 v21 = v2 + ratio * (v1 - v2);
        Vector3 v20 = v2 + ratio * (v0 - v2);
        Vector3 v02 = v0 + ratio * (v2 - v0);

        int indexFirstVertice = m_vertices.Count;
        m_vertices.Add(v01);
        m_vertices.Add(v10);
        m_vertices.Add(v12);
        m_vertices.Add(v21);
        m_vertices.Add(v20);
        m_vertices.Add(v02);

        int i01 = indexFirstVertice + 0;
        int i10 = indexFirstVertice + 1;
        int i12 = indexFirstVertice + 2;
        int i21 = indexFirstVertice + 3;
        int i20 = indexFirstVertice + 4;
        int i02 = indexFirstVertice + 5;

        m_triangles[indiceIndexTriangle + 0] = i12;
        m_triangles[indiceIndexTriangle + 1] = i20;
        m_triangles[indiceIndexTriangle + 2] = i01;

        AddTriangle(i10, i1, i12);
        AddTriangle(i20, i21, i2);
        AddTriangle(i0, i01, i02);

        AddTriangle(i01, i10, i12);
        AddTriangle(i12, i21, i20);
        AddTriangle(i20, i02, i01);

        if(fillGaps)
        {
            AddTriangle(i1, i10, i0);
            AddTriangle(i10, i01, i0);

            AddTriangle(i2, i21, i1);
            AddTriangle(i21, i12, i1);

            AddTriangle(i0, i02, i2);
            AddTriangle(i02, i20, i2);
        }
    }

    private void AddTriangle(int i0, int i1, int i2)
    {
        m_triangles.Add(i0);
        m_triangles.Add(i1);
        m_triangles.Add(i2);
    }

    private void MoveTowardsEdge(int indiceIndexVertice, int indiceIndexEdge, float ratio )
    {
        Vector3 v0 = m_vertices[m_triangles[indiceIndexVertice]];
        Vector3 v1 = m_vertices[m_triangles[indiceIndexEdge]];

        m_vertices[m_triangles[indiceIndexVertice]] = v0 + ratio * (v1 - v0);

    }


}
