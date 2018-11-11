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
    
	void Update ()
    {
        Plane cutPlane = new Plane(cuttingPlane.transform.up, cuttingPlane.transform.position);

        List<Vector3> vertices = new List<Vector3>(insideMesh.vertices);
        int[] indices = insideMesh.GetIndices(0);

        List<int> newIndices = new List<int>();

        Ray ray = new Ray(Vector3.zero, Vector3.down);


        /*for ( int i = 0; i < indices.Length; ++i)
        {
            if (cutPlane.GetSide(vertices[indices[i]]))
            {
                ray.origin = vertices[indices[i]];
                float distance;
                if (cutPlane.Raycast(ray, out distance))
                    vertices[indices[i]].y = ray.GetPoint(distance).y;
            }                
            newIndices.Add(indices[i]);
        }*/

        int[] cuts = new int[3];
        for (int i = 0; i < indices.Length/3; ++i)
        {
            int nbCut = 0;
            for (int j = 0; j < 3; ++j)
            {
                int index = i * 3 + j;

                if (cutPlane.GetSide(vertices[indices[index]]))                
                    cuts[ nbCut ++ ] = j; 
            }

            if(nbCut == 0)
            {
                newIndices.Add(indices[i * 3 + 0]);
                newIndices.Add(indices[i * 3 + 1]);
                newIndices.Add(indices[i * 3 + 2]);
            }
            else if(nbCut == 1)
            {
                Vector3 p1 = vertices[indices[i * 3 + cuts[0]]];
                Vector3 p2 = vertices[indices[i * 3 + cuts[0]]] + 0.1f*Vector3.up; 
                Debug.DrawLine(
                    m_gameObject.transform.position + m_gameObject.transform.rotation * p1,
                    m_gameObject.transform.position + m_gameObject.transform.rotation * p2,
                    Color.red 
                    );
            }



        }

        m_mesh.vertices = vertices.ToArray();
        m_mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);
    }

    void UpdateGeometry()
    {

    }
}
