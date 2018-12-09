using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Terraformation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SphereCollider sphereCol;

    [Header("Parameters")]
    [SerializeField, Range(0f, 1f)] private float m_speed = 0.01f;
    [SerializeField, Range(0f, 1f)] private float m_minDiameter = 0f;
    [SerializeField, Range(0f, 2f)] private float m_maxDiameter = 1f;

    private Renderer m_rend;
    private Mesh m_mesh;
    private Vector3[] m_normals;

    // Use this for initialization
    void Awake ()
    {
        m_mesh = GetComponent<MeshFilter>().mesh;
        m_rend = GetComponent<Renderer>();

        // Set normals
        m_normals = new Vector3[m_mesh.normals.Length];
        for (int i = 0; i < m_mesh.vertices.Length; i++)
        {
            m_normals[i] = m_mesh.vertices[i].normalized;
        }

        OnValidate();
    }

    private void OnValidate()
    {
        if(m_rend)
        {
            m_rend.material.SetFloat("_MinDiameter", m_minDiameter);
            m_rend.material.SetFloat("_MaxDiameter", m_maxDiameter);
        }
    }

    Ray m_ray = new Ray();
    void Update()
    {
        Vector3[] vertices = m_mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float magnitude = vertices[i].magnitude;


            RaycastHit hit;
            m_ray.direction = m_normals[i];
            m_ray.origin = Vector3.zero;
            if(sphereCol.Raycast(m_ray, out hit, magnitude))
            {
                if(hit.point.sqrMagnitude < vertices[i].sqrMagnitude)
                {
                    float scaleFactor = (hit.point - sphereCol.transform.position).sqrMagnitude;
                    vertices[i] = Mathf.Max(m_minDiameter, magnitude - m_speed * scaleFactor) * m_normals[i];
                }
            }
        }

        m_mesh.vertices = vertices;

        m_mesh.RecalculateNormals();
    }


}
