using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Terraformation : MonoBehaviour
{
    private Mesh m_mesh;

    // Use this for initialization
    void Awake ()
    {
        m_mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {

        Vector3[] vertices = m_mesh.vertices;
        Vector3[] normals = m_mesh.normals;

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] += 0.001f * normals[i] * Mathf.Sin(Time.time);
        }

        m_mesh.vertices = vertices;
    }
}
