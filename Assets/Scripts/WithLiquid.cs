using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithLiquid : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Mesh insideMesh = null;
    [SerializeField] private Material material = null;
    [SerializeField,] private float minHeight = 0f;
    [SerializeField,] private float maxHeight = 2.55f;
    [SerializeField, Range(0, 3f)] private float liquidHeight = 0.5f;
    [SerializeField] private bool containerClosed = true;

    [Header("Debug & Performance")]    
    [SerializeField] private float executionDelay = 0f;
    [SerializeField] private bool showDebugLines = false;
    [SerializeField] float executionTime;   // Raw perf measurement

    // Liquid mesh references
    private GameObject m_gameObject = null;
    private MeshRenderer m_meshRenderer = null;
    private MeshFilter m_meshFilter = null;
    private Mesh m_mesh = null;

    // Mesh data
    private Vector3[] m_baseVertices;       // Vertices of the mesh to cut 
    private int[] m_baseIndices;            // indices of the mesh to cut 
    Vector3[] newVertices;                  // Vertices of the final cut mesh  
    List<int> newIndices = new List<int>(); // Indices of the final cut mesh  

    // Usefull tools
    Plane cutPlane = new Plane();   // Cut plane
    Ray ray = new Ray();            // Usefull for plane raycast

    // Cutting algorithm
    int[] cuts = new int[3];            // Indices of vertices cut on a single triangle    
    int m_additionalVerticesCount = 0;  // Count of additionnal vertices created

    // Matching data structures
    int nbSets = 0;                                             // Count of connex vertices edges
    List<HashSet<int>> matchSet = new List<HashSet<int>>();     // List of HashSet for matching vertices 
    List<List<Vector3>> matchEdges = new List<List<Vector3>>(); // List of Vector3 for saving matched edges
    List<HashSet<int>> sets = new List<HashSet<int>>();         // List of vertex hash in a connex group
    List<List<Vector3>> edges = new List<List<Vector3>>();      // List of edges of the same connex group

    // Other
    private float m_timeSinceLastCall = 0f;    // raw perf measurement
    Color[] colors = { Color.yellow, Color.green, Color.blue, Color.red, Color.magenta, Color.cyan };// Debug lines colors

    // Init
    void Awake ()
    {
        // new GameObject
        m_gameObject = new GameObject("LiquidMesh");
        m_gameObject.transform.position = transform.position + minHeight * Vector3.up;
        m_gameObject.transform.parent = transform;
        m_gameObject.transform.localScale = new Vector3(1,1,1);

        // Mesh & utility
        m_mesh = new Mesh();
        m_mesh.name = "liquidMesh";
        m_mesh.MarkDynamic();

        m_meshFilter = m_gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;

        m_meshRenderer = m_gameObject.AddComponent<MeshRenderer>();
        if (material)
            m_meshRenderer.material = material;

        // Init data 
        for( int i = 0; i < 30; ++i)
        {
            sets.Add( new HashSet<int>());
            edges.Add( new List<Vector3>());
        }

        m_baseIndices = insideMesh.GetIndices(0); ;
        m_baseVertices = insideMesh.vertices;

        newVertices = new Vector3[m_baseVertices.Length];
        m_baseVertices.CopyTo(newVertices, 0);
    }

    private void Start()
    {
        float h = top.position.y - bot.position.y;
        liquidHeight = h / 2;
    }

    // Update is called once per frame
    void Update ()
    {
        // Calls Update every executionDelay
        m_timeSinceLastCall += Time.deltaTime;
        if (m_timeSinceLastCall > executionDelay)
        {
            float t = Time.realtimeSinceStartup;
            if (liquidHeight > 0)
            {
                BuildEges();
                BuildTopFace();
                UpdateMesh();
                m_meshRenderer.enabled = true;
            }
            else
                m_meshRenderer.enabled = false;

 
            executionTime = 1000f * (Time.realtimeSinceStartup - t);
            m_timeSinceLastCall = 0;
        }
    }

    void BuildEges()
    {
        highestEdge = Vector3.negativeInfinity;
        /*float h = top.position.y - bot.position.y;
        if (liquidHeight > h)
            liquidHeight = h;*/

        Vector3 botPoint = center.position - Vector3.Distance(center.position,bot.position) * Vector3.up;
        Vector3 meshSpaceBotPoint = Quaternion.Inverse(transform.rotation) * botPoint;
        Vector3 normal = Quaternion.Inverse(transform.rotation) * Vector3.up;
        Vector3 p = meshSpaceBotPoint + normal * liquidHeight;
        RelativeDebugLine(meshSpaceBotPoint, p, Color.red); 

        cutPlane.SetNormalAndPosition(normal, p);

        newIndices.Clear();
        float distance;
        m_additionalVerticesCount = 0;
        for (int i = 0; i < sets.Count; ++i)
        {
            sets[i].Clear();
            edges[i].Clear();
        }
        nbSets = 0;

        for (int i = 0; i < m_baseIndices.Length / 3; ++i)
        {
            int nbCut = 0;
            for (int j = 0; j < 3; ++j)
            {
                int index = i * 3 + j;     
                if (cutPlane.GetSide(m_baseVertices[m_baseIndices[index]]))
                    cuts[nbCut++] = j;
                else
                    cuts[2] = j;
            }

            // No cut 
            if (nbCut == 0)
            {
                newIndices.Add(m_baseIndices[i * 3 + 0]);
                newIndices.Add(m_baseIndices[i * 3 + 1]);
                newIndices.Add(m_baseIndices[i * 3 + 2]);

                CheckHighest(m_baseVertices[ m_baseIndices[i * 3 + 0]]);
                CheckHighest(m_baseVertices[ m_baseIndices[i * 3 + 1]]);
                CheckHighest(m_baseVertices[ m_baseIndices[i * 3 + 2]]);
            }
            // 1 edge cut
            else if (nbCut == 1)
            {
                Vector3 p1 = m_baseVertices[m_baseIndices[i * 3 + cuts[0]]];
                Vector3 p2 = m_baseVertices[m_baseIndices[i * 3 + (cuts[0] + 1) % 3]];
                Vector3 p3 = m_baseVertices[m_baseIndices[i * 3 + (cuts[0] + 3 - 1) % 3]];

                Vector3 proj1 = Vector3.zero;
                Vector3 proj2 = Vector3.zero;

                ray.origin = p1;
                ray.direction = p2 - p1;
                if (cutPlane.Raycast(ray, out distance))
                {
                    proj1 = ray.GetPoint(distance);
                    RelativeDebugLine(proj1, p2, Color.red);
                    RelativeDebugLine(p2, p3, Color.red);
                    RelativeDebugLine(p3, proj1, Color.red);

                    // Adds the new vertice and triangle
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    newIndices.Add(m_baseIndices[i * 3 + (cuts[0] + 1) % 3]);       //p2
                    newIndices.Add(m_baseIndices[i * 3 + (cuts[0] + 3 - 1) % 3]);   // p3
                    AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, proj1);
                    ++m_additionalVerticesCount;
                }

                ray.origin = p1;
                ray.direction = p3 - p1;
                if (cutPlane.Raycast(ray, out distance))
                {
                    proj2 = ray.GetPoint(distance);

                    // Adds the new vertice and triangle
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount - 1);

                    newIndices.Add(m_baseIndices[i * 3 + (cuts[0] + 3 - 1) % 3]);//P3
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, proj2);
                    ++m_additionalVerticesCount;


                    RelativeDebugLine(proj2, p3, Color.red);
                    RelativeDebugLine(p3, proj1, Color.red);
                }
                Match(proj1, proj2);

            }
            // 2 edges cut
            else if (nbCut == 2)
            {
                Vector3 p1 = m_baseVertices[m_baseIndices[i * 3 + cuts[0]]];
                Vector3 p2 = m_baseVertices[m_baseIndices[i * 3 + cuts[1]]];
                Vector3 p3 = m_baseVertices[m_baseIndices[i * 3 + cuts[2]]];

                Vector3 proj1 = Vector3.zero;
                Vector3 proj2 = Vector3.zero;

                ray.origin = p1;
                ray.direction = p3 - p1;
                if (cutPlane.Raycast(ray, out distance))
                {
                    proj1 = ray.GetPoint(distance);
                    RelativeDebugLine(p3, proj1, Color.blue);
                }

                ray.origin = p2;
                ray.direction = p3 - p2;
                if (cutPlane.Raycast(ray, out distance))
                {
                    proj2 = ray.GetPoint(distance);
                    RelativeDebugLine(proj2, p3, Color.blue);
                }
                Match(proj1, proj2);

                // Check the triangle orientation
                Vector3 norm1 = Vector3.Cross(
                    m_baseVertices[m_baseIndices[i * 3 + 1]] - m_baseVertices[m_baseIndices[i * 3 + 0]],
                    m_baseVertices[m_baseIndices[i * 3 + 2]] - m_baseVertices[m_baseIndices[i * 3 + 0]]);
                Vector3 norm2 = Vector3.Cross(proj2 - proj1, p3 - proj1);
                if( Vector3.Dot(norm1, norm2) > 0)
                {
                    // Adds triangles indices
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 1);
                    newIndices.Add(m_baseIndices[i * 3 + cuts[2]]); // P3
                }
                else
                {
                    // Adds triangles indices
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    newIndices.Add(m_baseIndices[i * 3 + cuts[2]]); // P3
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 1);
                }

                // Adds projected vertices
                AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount,proj1);
                ++m_additionalVerticesCount;
                AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, proj2);
                ++m_additionalVerticesCount;
            }
        }
    }

    void CheckHighest(Vector3 point)
    {
        if (point.y > highestEdge.y)
            highestEdge = point;
    }

    Vector3 highestEdge = Vector3.negativeInfinity;
    void BuildTopFace()
    {  
        // Draw the topFace
        for (int j = 0; j < edges.Count; ++j)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < edges[j].Count / 2; ++i)
                center += edges[j][2 * i];
            center /= edges[j].Count / 2;

            RelativeDebugLine(center, center + 0.3f * cutPlane.normal, colors[j % colors.Length]);
            for (int i = 0; i < edges[j].Count / 2; ++i)
            {
                CheckHighest(edges[j][2 * i]);

                RelativeDebugLine(edges[j][2 * i], edges[j][2 * i + 1], colors[j % colors.Length]);

                Vector3 normal = Vector3.Cross(center - edges[j][2 * i], center - edges[j][2 * i + 1]);

                if (Vector3.Dot(normal, cutPlane.normal)> 0)
                {
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 1);
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 2);
                }
                else
                {
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 1);
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount);
                    newIndices.Add(m_baseVertices.Length + m_additionalVerticesCount + 2);
                }

                AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, edges[j][2 * i]);
                ++m_additionalVerticesCount;
                AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, edges[j][2 * i + 1]);
                ++m_additionalVerticesCount;
                AddNewVertice(m_baseVertices.Length + m_additionalVerticesCount, center);
                ++m_additionalVerticesCount;
            }
        }

        RelativeDebugLine(highestEdge, highestEdge + Vector3.up, Color.blue);

        //
        //RelativeDebugLine(highestEdge, highestEdge + cutPlane.normal, Color.red);
         if ( !containerClosed)
         {
             if (highestEdge.y > maxHeight)
             {               
                 Vector3 p = Quaternion.Inverse(transform.rotation) * (highestEdge);
                 RelativeDebugLine(p, p + 2*cutPlane.normal, Color.blue);

                 liquidHeight = Mathf.Max(0f, liquidHeight - speed * Time.deltaTime);
             }
         }

    }

    [SerializeField] float speed = 0.01f;
    public Transform top;
    public Transform center;
    public Transform bot;

    void UpdateMesh()
    {
        m_mesh.Clear();
        m_mesh.vertices = newVertices;
        m_mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);
        m_mesh.RecalculateNormals();
    }
   
    //  Match an edge with the others connex groups of edges
    void Match(Vector3 proj1, Vector3 proj2)
    {
        matchSet.Clear();
        matchEdges.Clear();
        for (int setIndex = 0; setIndex < nbSets; ++setIndex)
        {
            if (sets[setIndex].Count == 0 || sets[setIndex].Contains(proj1.GetHashCode()) || sets[setIndex].Contains(proj2.GetHashCode()))
            {
                matchSet.Add(sets[setIndex]);
                matchEdges.Add(edges[setIndex]);
            }
        }

        if (matchSet.Count == 0)
        {
            sets[nbSets].Add(proj1.GetHashCode());
            sets[nbSets].Add(proj2.GetHashCode());
            edges[nbSets].Add(proj1);
            edges[nbSets].Add(proj2);
            nbSets++;
        }
        else
        {
            matchSet[0].Add(proj1.GetHashCode());
            matchSet[0].Add(proj2.GetHashCode());
            matchEdges[0].Add(proj1);
            matchEdges[0].Add(proj2);

            for (int i = 1; i < matchSet.Count; ++i)
            {
                matchSet[0].UnionWith(matchSet[i]);
                matchEdges[0].AddRange(matchEdges[i]);
            }
            while (matchSet.Count > 1)
            {
                List<Vector3> oldEdges = matchEdges[1];
                HashSet<int> oldSet = matchSet[1];
                sets.Remove(oldSet);
                edges.Remove(oldEdges);
                oldSet.Clear();
                oldEdges.Clear();
                sets.Add(oldSet);
                edges.Add(oldEdges);
                matchSet.RemoveAt(1);
                nbSets--;
            }
        }
    }

    // Add a new vertice to ther newVertices array with error checking
    void AddNewVertice(int index ,  Vector3 vertice)
    {
        try
        {
            newVertices[index] = vertice;
        }
        catch(IndexOutOfRangeException)
        {
            Vector3[] newArray = new Vector3[2 * newVertices.Length];
            newVertices.CopyTo(newArray, 0);
            newVertices = newArray;
            AddNewVertice(index, vertice);
        }
    }

    // Draws a debug line relative to the liquidMesh gameobject
    void RelativeDebugLine(Vector3 start, Vector3 end, Color color)
    {
        if (showDebugLines)
        {
            Debug.DrawLine(
            m_gameObject.transform.position + m_gameObject.transform.rotation * start,
            m_gameObject.transform.position + m_gameObject.transform.rotation * end,
            color
            );
        }
    }

    private void OnValidate()
    {
        if (m_gameObject)
            m_gameObject.transform.position = transform.position + minHeight * Vector3.up;
    }
}
