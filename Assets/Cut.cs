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
    public bool test = true;
	void Update ()
    {
        Plane cutPlane = new Plane(cuttingPlane.transform.up, cuttingPlane.transform.position);

        List<Vector3> vertices = new List<Vector3>(insideMesh.vertices);
        int[] indices = insideMesh.GetIndices(0);

        List<int> newIndices = new List<int>();
        List<int> edges = new List<int>();

        Ray ray = new Ray(Vector3.zero, Vector3.down);
        float distance;

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
            else if (nbCut == 1 && test)
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
                    RelativeDebugLine(proj1, proj2, Color.yellow);//h
                    RelativeDebugLine(proj2, p3, Color.red);
                    RelativeDebugLine(p3, proj1, Color.red);
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

                 RelativeDebugLine(proj1, proj2, Color.yellow);//h*/
            }
        }

        List<List<int>> l = new List<List<int>>();


        m_mesh.vertices = vertices.ToArray();
        m_mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);
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
