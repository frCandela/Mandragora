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
        if (heightLiquid > 0)
        {
            Vector3 containerRotation = transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(-containerRotation.x, 0, -containerRotation.z);
            Plane topPlane = new Plane(rotation * Vector3.up, heightLiquid * Vector3.up);
            Plane botPlane = new Plane(Vector3.up, Vector3.zero);
            float spillage = 0f;


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
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                vertices[i] = radius * ( dir).normalized + heightLiquid * Vector3.up;

                //Plane sidePlane = new Plane(-dir, radius * dir.normalized);
                float enter;
                Ray ray = new Ray(vertices[i], Vector3.up);
                if (topPlane.Raycast(ray, out enter))
                {
                    vertices[i] = ray.GetPoint(enter);  
                }
                else
                {
                    ray = new Ray(vertices[i], -Vector3.up);
                    if (topPlane.Raycast(ray, out enter))
                    {
                        vertices[i] = ray.GetPoint(enter);
                    }
                    else
                        print("Invalid topPlane raycast");
                }
            }
            for (int i = 0; i < sides; ++i)// Set indexes
            {
                indices[i * 3 + 0] = sides;
                indices[i * 3 + 1] = (i + 2) % (sides);
                indices[i * 3 + 2] = (i + 1) % (sides);
            }

            for (int i = 0; i < sides; ++i)// Set indexes
            {
                Vector3 vec1 = vertices[ indices[i * 3 + 1]];
                Vector3 vec2 = vertices[ indices[i * 3 + 2]];

                if(vec1.y < 0)
                {
                    float enter;
                    Ray ray = new Ray(vec1, vec2 - vec1);
                    if (botPlane.Raycast(ray, out enter))
                    {
                        vertices[indices[i * 3 + 1]] = ray.GetPoint(enter);
                    }
                }
                if (vec2.y < 0)
                {
                    float enter;
                    Ray ray = new Ray(vec2, vec1 - vec2);
                    if (botPlane.Raycast(ray, out enter))
                    {
                        vertices[indices[i * 3 + 2]] = ray.GetPoint(enter);
                    }
                }

                if( vec1.y > heightContainer)
                {
                    spillage = Mathf.Max(spillage, vec1.y-heightContainer);
                }


                Debug.DrawLine(
                    transform.position + transform.rotation * vertices[indices[i * 3 + 1]],
                    transform.position + transform.rotation * vertices[indices[i * 3 + 2]],
                    Color.red
                    );

            }

            print(spillage);
            heightLiquid = Mathf.Max(0, heightLiquid - spillage);


            /*
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



                }


            }*/
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
