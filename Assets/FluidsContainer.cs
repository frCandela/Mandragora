using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidsContainer : MonoBehaviour
{
    [SerializeField] private Material fluidMaterial = null;
    [SerializeField] private int sides = 4;    // cubic
    [SerializeField] private float radius = 0.5f;
    [SerializeField, Range(0f,1f)] private float height = 0f;

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

        UpdateGeometry();
    }

    void UpdateGeometry()
    {
        m_liquidMesh.Clear();

        Vector3[] vertices = new Vector3[2*(sides + 1)];
        int[] indices = new int[2*(3 * sides)];
        int offsetVertices = 0;
        int offsetIndices = 0;

        // Creates the top fluid mesh face ( polygon with sides sides)
        vertices[sides + offsetVertices] = new Vector3(0, height, 0);
        for (int i = 0; i < sides; ++i)
        {
            float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
            vertices[i + offsetVertices] =  new Vector3(radius * Mathf.Cos(angle), height, radius * Mathf.Sin(angle));
        }
        offsetVertices += sides + 1;
        for (int i = 0; i < sides; ++i)
        {
            indices[i * 3 + 0 + offsetIndices] = sides + offsetIndices;
            indices[i * 3 + 1 + offsetIndices] = (i + 2) % (sides) + offsetIndices;
            indices[i * 3 + 2 + offsetIndices] = (i + 1) % (sides) + offsetIndices; 
        }
        offsetIndices += 3*sides;


        // Creates the bottom fluid mesh face ( polygon with sides sides)
        vertices[sides + offsetVertices] = new Vector3(0, -height, 0);
        for ( int i = 0; i < sides; ++i)
        {
            float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
            vertices[i + offsetVertices] =  new Vector3(radius * Mathf.Cos(angle), -height, radius * Mathf.Sin(angle));
        }
        for (int i = 0; i < sides; ++i)
        {
            indices[i * 3 + 0 + offsetIndices] = sides + offsetVertices;
            indices[i * 3 + 1 + offsetIndices] = (i+1) % (sides) + offsetVertices;
            indices[i * 3 + 2 + offsetIndices] = (i+2) % (sides) + offsetVertices;
        }

        m_liquidMesh.vertices = vertices;
        m_liquidMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateGeometry();

    }
}
