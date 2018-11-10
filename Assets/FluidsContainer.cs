using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidsContainer : MonoBehaviour
{
    [SerializeField] private Material fluidMaterial = null;
    [SerializeField, Range(3,  20)] private int   sides = 4;    // cubic
    [SerializeField, Range(0f, 1f)] private float radius = 0.5f;
    [SerializeField, Range(0f, 1f)] private float heightContainer = 1f;
    [SerializeField, Range(0f, 1f)] private float heightLiquid = 0.5f;


    [SerializeField, Range(-90, 90)] private float angleX = 0f;
    [SerializeField, Range(-90, 90)] private float angleZ = 0f;

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
        if( heightLiquid > 0)
        {
            Vector3[] vertices = new Vector3[2 * (sides + 1)];
            int[] indices = new int[2 * (3 * sides) + 2 * 3 * sides];
            int offsetVerticesBot = sides + 1;
            int offsetIndicesBot = 3 * sides;
            int offsetVerticesSides = offsetVerticesBot + sides + 1;
            int offsetIndicesSides = offsetIndicesBot + 3 * sides;

            // Creates the top fluid mesh face ( polygon with sides sides)
            vertices[sides] = new Vector3(0, heightLiquid, 0);
            for (int i = 0; i < sides; ++i)
            {
                float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
                vertices[i] = new Vector3(radius * Mathf.Cos(angle), heightLiquid, radius * Mathf.Sin(angle));
            }
            for (int i = 0; i < sides; ++i)// Set indexes
            {
                indices[i * 3 + 0] = sides;
                indices[i * 3 + 1] = (i + 2) % (sides);
                indices[i * 3 + 2] = (i + 1) % (sides);
            }

            // Creates the bottom fluid mesh face ( polygon with sides sides)
            vertices[sides + offsetVerticesBot] = new Vector3(0, 0, 0);
            for (int i = 0; i < sides; ++i)
            {
                float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
                vertices[i + offsetVerticesBot] = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            }
            for (int i = 0; i < sides; ++i)// Set indexes
            {
                indices[i * 3 + 0 + offsetIndicesBot] = sides + offsetVerticesBot;
                indices[i * 3 + 1 + offsetIndicesBot] = (i + 1) % sides + offsetVerticesBot;
                indices[i * 3 + 2 + offsetIndicesBot] = (i + 2) % sides + offsetVerticesBot;
            }

            // Creates sides
            for (int i = 0; i < sides; ++i)
            {
                indices[offsetIndicesSides + 6 * i + 0] = i + 0;
                indices[offsetIndicesSides + 6 * i + 1] = (i + 1) % sides;
                indices[offsetIndicesSides + 6 * i + 2] = offsetVerticesBot + i % sides;

                indices[offsetIndicesSides + 6 * i + 3] = (i + 1) % sides;
                indices[offsetIndicesSides + 6 * i + 4] = offsetVerticesBot + (i + 1) % sides;
                indices[offsetIndicesSides + 6 * i + 5] = offsetVerticesBot + (i + 0) % sides;
            }


            // Creates the top fluid mesh face ( polygon with sides sides)
            float maxHeight = 0f;
            Vector3 containerRotation = transform.rotation.eulerAngles;
            Quaternion rot = Quaternion.Euler(-containerRotation.x, 0, -containerRotation.z);
            Plane topPlane = new Plane(rot * Vector3.up, heightLiquid * Vector3.up);
            Plane botPlane = new Plane(rot * Vector3.up, Vector3.zero);

            vertices[sides] = new Vector3(0, heightLiquid, 0);
            for (int i = 0; i < sides; ++i)
            {
                float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
                Ray ray = new Ray(vertices[i], Vector3.up);
                float enter;
                if (topPlane.Raycast(ray, out enter)) // Raycast up
                {
                    vertices[i] = ray.GetPoint(enter);
                    maxHeight = Mathf.Max(maxHeight, enter);
                }
                else // Raycast Down
                {
                    ray = new Ray(vertices[i], -Vector3.up);
                    if (topPlane.Raycast(ray, out enter))
                    {
                        vertices[i] = ray.GetPoint(enter);
                        maxHeight = Mathf.Max(maxHeight, enter);

                        if (vertices[i].y < 0)
                        {
                            //Out of container
                            /* Plane plane2 = new Plane(rot * Vector3.up, Vector3.zero);
                             ray = new Ray(vertices[i], -vertices[i]);
                             if (plane2.Raycast(ray, out enter))
                             {
                                 print("zob");
                                 vertices[i] = ray.GetPoint(enter);
                             }*/
                        }


                    }
                }
            }

            // Liquid overflow
            if (heightLiquid + maxHeight > heightContainer)
                heightLiquid -= heightLiquid + maxHeight - heightContainer;

            // Creates the bottom fluid mesh face ( polygon with sides sides)
            vertices[sides + offsetVerticesBot] = new Vector3(0, 0, 0);
            for (int i = 0; i < sides; ++i)
            {
                float angle = i * 2 * Mathf.PI / (sides) + Mathf.PI / sides;
                // Surface of liquid is below the vertex
                Ray ray = new Ray(vertices[i + offsetVerticesBot], Vector3.down);
                float enter;
                if (topPlane.Raycast(ray, out enter))
                {
                    Plane planeBot = new Plane(rot * Vector3.up, Vector3.zero);
                    Vector3 dir = transform.forward;
                    dir.y = 0;

                    Debug.DrawLine(ray.GetPoint(enter), vertices[i + offsetVerticesBot]);

                    /* ray = new Ray(ray.GetPoint(enter), -Vector3.forward);
                     if (planeBot.Raycast(ray, out enter))
                     {
                         print("zob");
                         vertices[i] = ray.GetPoint(enter);
                     }*/



                    /*ray = new Ray(vertices[i + offsetVerticesBot], -vertices[i + offsetVerticesBot]);
                    if (plane.Raycast(ray, out enter))
                    {
                        vertices[i + offsetVerticesBot] = ray.GetPoint(enter);
                    }*/

                }


            }

            m_liquidMesh.vertices = vertices;
            m_liquidMesh.SetIndices(indices, MeshTopology.Triangles, 0);

            m_liquidMesh.vertices = vertices;
            m_liquidMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateGeometry();

    }
}
