using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidsContainer : MonoBehaviour
{
    [SerializeField] private Material fluidMaterial = null;
    [SerializeField] private int containerSides = 4;    // cubic
    [SerializeField] private float radius = 0.5f;    // cubic

    private MeshRenderer m_meshRenderer = null;
    private MeshFilter m_meshFilter = null;
    private Mesh m_liquidMesh = null;

    // Use this for initialization
    void Awake ()
    {
        m_liquidMesh = new Mesh();
        m_liquidMesh.name = "liquidMesh";

        m_meshFilter = gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_liquidMesh;

        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (fluidMaterial)
            m_meshRenderer.material = fluidMaterial;

        createGeometry();
    }

    void createGeometry()
    {
        m_liquidMesh.Clear();

        Vector3[] vertices = new Vector3[containerSides + 1];
        vertices[containerSides] = new Vector3(0, 0, 0);
        for ( int i = 0; i < containerSides; ++i)
        {
            float angle = i * 2 * Mathf.PI / (containerSides) + Mathf.PI / containerSides;
            vertices[i] = radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }
        m_liquidMesh.vertices = vertices;


        int[] indices = new int[ 3 * containerSides ];
        for (int i = 0; i < containerSides; ++i)
        {
            indices[i * 3 + 0] = containerSides;
            indices[i * 3 + 1] = (i+1) % (containerSides);
            indices[i * 3 + 2] = (i+2) % (containerSides);

        }
        m_liquidMesh.SetIndices(indices, MeshTopology.Triangles, 0);

        for (int i = 0; i < indices.Length; ++i)
            print(indices[i]);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
