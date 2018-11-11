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
        m_gameObject.transform.position = transform.position + 2*Vector3.up;

        m_mesh = new Mesh();
        m_mesh.name = "liquidMesh";

        m_meshFilter = m_gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;

        m_meshRenderer = m_gameObject.AddComponent<MeshRenderer>();
        if (material)
            m_meshRenderer.material = material;
    }
	
	// Update is called once per frame
    
	void Update ()
    {
        Plane cutPlanePlane = new Plane(cuttingPlane.transform.position, cuttingPlane.transform.up);

        Vector3[] vertices = insideMesh.vertices;
        int[] indices = insideMesh.GetIndices(0);

        m_mesh.vertices = vertices;
        m_mesh.SetIndices(indices, MeshTopology.Triangles, 0);

    }

    void UpdateGeometry()
    {

    }
}
