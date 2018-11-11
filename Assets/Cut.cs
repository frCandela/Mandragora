using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    [SerializeField] private Transform cuttingPlane = null;
    [SerializeField] private Mesh insideMesh = null;
    [SerializeField] private Material material = null;

    private GameObject m_gameObject = null;
    private MeshRenderer m_meshRenderer = null;
    private MeshFilter m_meshFilter = null;
    private Mesh m_mesh = null;

    [SerializeField] float duration;


    // Use this for initialization
    void Awake ()
    {
        m_gameObject = new GameObject("Test");
        m_gameObject.transform.position = transform.position + 3*Vector3.up;

        m_mesh = new Mesh();
        m_mesh.name = "liquidMesh";
        m_mesh.MarkDynamic();

        m_meshFilter = m_gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;

        m_meshRenderer = m_gameObject.AddComponent<MeshRenderer>();
        if (material)
            m_meshRenderer.material = material;
    }


    // Update is called once per frame
	void Update ()
    {
        float t = Time.realtimeSinceStartup;

        Plane cutPlane = new Plane(cuttingPlane.transform.up, cuttingPlane.transform.position);

        List<Vector3> vertices = new List<Vector3>(insideMesh.vertices);
        int[] indices = insideMesh.GetIndices(0);

        List<int> newIndices = new List<int>();

        Ray ray = new Ray(Vector3.zero, Vector3.down);
        float distance;

        Color[] colors = { Color.yellow, Color.green , Color.blue , Color.red , Color.magenta , Color.cyan};
        List<HashSet<int>> sets = new List<HashSet<int>>();
        List <List<Vector3 >> edges = new List<List<Vector3>>();
        sets.Add(new HashSet<int>());
        edges.Add(new List<Vector3>());

        int[] cuts = new int[3];
        for (int i = 0; i < indices.Length / 3; ++i)
        {
            int nbCut = 0;
            for (int j = 0; j < 3; ++j)
            {
                int index = i * 3 + j;

                if (cutPlane.GetSide(vertices[indices[index]]))
                    cuts[nbCut++] = j;
                else
                    cuts[2] = j;
            }

            if (nbCut == 0)
            {
                newIndices.Add(indices[i * 3 + 0]);
                newIndices.Add(indices[i * 3 + 1]);
                newIndices.Add(indices[i * 3 + 2]);
            }
            else if (nbCut == 1)
            {
                Vector3 p1 = vertices[indices[i * 3 + cuts[0]]];
                Vector3 p2 = vertices[indices[i * 3 + (cuts[0] + 1) % 3]];
                Vector3 p3 = vertices[indices[i * 3 + (cuts[0] + 3 - 1) % 3]];

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
                }

                ray.origin = p1;
                ray.direction = p3 - p1;
                if (cutPlane.Raycast(ray, out distance))
                {
                    proj2 = ray.GetPoint(distance);
                    //RelativeDebugLine(proj1, proj2, Color.yellow);//////////////////////////////////////////////////////////////
                    RelativeDebugLine(proj2, p3, Color.red);
                    RelativeDebugLine(p3, proj1, Color.red);
                }

                List<int> matchSet = new List<int>();
                for (int setIndex = 0 ; setIndex < sets.Count; ++setIndex)
                {
                    if (sets[setIndex].Count == 0 || sets[setIndex].Contains(proj1.GetHashCode()) || sets[setIndex].Contains(proj2.GetHashCode()))                    
                        matchSet.Add(setIndex);                    
                }

                if (matchSet.Count == 0)
                {
                    sets.Add(new HashSet<int>());
                    edges.Add(new List<Vector3>());
                    sets[sets.Count - 1].Add(proj1.GetHashCode());
                    sets[sets.Count - 1].Add(proj2.GetHashCode());
                    edges[edges.Count - 1].Add(proj1);
                    edges[edges.Count - 1].Add(proj2);
                }
                else
                {
                    sets[matchSet[0]].Add(proj1.GetHashCode());
                    sets[matchSet[0]].Add(proj2.GetHashCode());
                    edges[matchSet[0]].Add(proj1);
                    edges[matchSet[0]].Add(proj2);
                    while (matchSet.Count > 1)
                    {
                        sets[matchSet[0]].UnionWith(sets[matchSet[1]]);
                        edges[matchSet[0]].AddRange(edges[matchSet[1]]);
                        sets.RemoveAt(matchSet[1]);
                        edges.RemoveAt(matchSet[1]);
                        matchSet.RemoveAt(1);
                    }
                }
            }
            else if (nbCut == 2)
            {
                Vector3 p1 = vertices[indices[i * 3 + cuts[0]]];
                Vector3 p2 = vertices[indices[i * 3 + cuts[1]]];
                Vector3 p3 = vertices[indices[i * 3 + cuts[2]]];

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
                List<int> matchSet = new List<int>();
                for (int setIndex = 0; setIndex < sets.Count; ++setIndex)
                {
                    if (sets[setIndex].Count == 0 || sets[setIndex].Contains(proj1.GetHashCode()) || sets[setIndex].Contains(proj2.GetHashCode()))
                        matchSet.Add(setIndex);
                }

                if (matchSet.Count == 0)
                {
                    sets.Add(new HashSet<int>());
                    edges.Add(new List<Vector3>());
                    sets[sets.Count - 1].Add(proj1.GetHashCode());
                    sets[sets.Count - 1].Add(proj2.GetHashCode());
                    edges[edges.Count - 1].Add(proj1);
                    edges[edges.Count - 1].Add(proj2);
                }
                else
                {
                    sets[matchSet[0]].Add(proj1.GetHashCode());
                    sets[matchSet[0]].Add(proj2.GetHashCode());
                    edges[matchSet[0]].Add(proj1);
                    edges[matchSet[0]].Add(proj2);
                    while (matchSet.Count > 1)
                    {
                        sets[matchSet[0]].UnionWith(sets[matchSet[1]]);
                        edges[matchSet[0]].AddRange(edges[matchSet[1]]);
                        sets.RemoveAt(matchSet[1]);
                        edges.RemoveAt(matchSet[1]);
                        matchSet.RemoveAt(1);
                    }
                }
                //RelativeDebugLine(proj1, proj2, Color.yellow);//////////////////////////////////////////////////////////////
            }
        }

        for(int j = 0; j < edges.Count; ++j)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < edges[j].Count / 2; ++i)            
                center += edges[j][2*i];
            center /= edges[j].Count/2;


            Vector3 normal = cutPlane.normal;
            RelativeDebugLine(center, center+0.3f* normal, colors[j % colors.Length]);

            for (int i = 0; i < edges[j].Count / 2; ++i)
            {
                RelativeDebugLine(edges[j][2 * i], edges[j][2 * i + 1], colors[j % colors.Length]);
            }
        }


        print(sets[0].Count);

        m_mesh.vertices = vertices.ToArray();
        m_mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);

        duration = 1000f*(Time.realtimeSinceStartup - t);
    }

    void RelativeDebugLine( Vector3 start, Vector3 end, Color color)
    {
        Debug.DrawLine(
        m_gameObject.transform.position + m_gameObject.transform.rotation * start,
        m_gameObject.transform.position + m_gameObject.transform.rotation * end,
        color
    );
    }

    void UpdateGeometry()
    {

    }
}
