using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icosphere : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float m_radius = 1f;
    [SerializeField, Range(0, 5)] private int m_nbSubdivisions = 0;
    [SerializeField] private bool m_showSegment = false;

    private Mesh m_mesh;

    void Start()
    {
        m_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_mesh;
    }

    // Update is called once per frame
    void Update( )
    {
        List<Vector3> vertices = GenerateIcosphere( m_radius, m_nbSubdivisions);
        if (m_showSegment)
        {
            vertices = GetSegment(vertices);
        }

        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < vertices.Count / 3; ++i)
        {
            Vector3 normal = Vector3.Cross(vertices[3 * i + 0] - vertices[3 * i + 1], vertices[3 * i + 1] - vertices[3 * i + 2]).normalized;
            normals.Add(normal); normals.Add(normal); normals.Add(normal);

            triangles.Add(3 * i + 0); triangles.Add(3 * i + 1); triangles.Add(3 * i + 2);
        }

        m_mesh.Clear();
        m_mesh.vertices = vertices.ToArray();
        m_mesh.triangles = triangles.ToArray();
        m_mesh.normals = normals.ToArray();
    }


    public static List<Vector3> GetSegment(List<Vector3> sampleVertices)
    {
        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(sampleVertices[0]);
        vertices.Add(sampleVertices[1]);
        vertices.Add(sampleVertices[2]);

        vertices.Add(sampleVertices[0]);
        vertices.Add(Vector3.zero);
        vertices.Add(sampleVertices[1]);

        vertices.Add(sampleVertices[1]);
        vertices.Add(Vector3.zero);
        vertices.Add(sampleVertices[2]);

        vertices.Add(sampleVertices[2]);
        vertices.Add(Vector3.zero);
        vertices.Add(sampleVertices[0]);

        return vertices;
    }

    public static List<Vector3> GenerateIcosphere(float radius, int subdivisions)
    {
        List<Vector3> vertices = new List<Vector3>();

        //float radius = length * Mathf.Sin(2 * Mathf.PI / 5);
        float angle1 = Mathf.PI / 2 - Mathf.Atan(1f / 2f);
        float angle2 = Mathf.Deg2Rad * 36f;

        float longitude = 0;
        float latitude = angle1;
        for (int i = 0; i < 5; ++i)
        {
            vertices.Add(SphericalCoord(longitude, latitude, radius));
            longitude += 2 * angle2;
            vertices.Add(SphericalCoord(longitude, latitude, radius));
            vertices.Add(radius * Vector3.up);
        }

        longitude = angle2;
        latitude = Mathf.PI - angle1;
        for (int i = 0; i < 5; ++i)
        {
            vertices.Add(SphericalCoord(longitude, latitude, radius));
            longitude += 2 * angle2;
            vertices.Add(-radius * Vector3.up);
            vertices.Add(SphericalCoord(longitude, latitude, radius));

        }

        longitude = 0f;
        for (int i = 0; i < 5; ++i)
        {
            latitude = Mathf.PI / 2 + Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v1 = SphericalCoord(longitude, latitude, radius);

            latitude = Mathf.PI / 2 - Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v2 = SphericalCoord(longitude, latitude, radius);

            latitude = Mathf.PI / 2 + Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v3 = SphericalCoord(longitude, latitude, radius);

            latitude = Mathf.PI / 2 - Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v4 = SphericalCoord(longitude, latitude, radius);

            latitude = Mathf.PI / 2 + Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v5 = SphericalCoord(longitude, latitude, radius);

            latitude = Mathf.PI / 2 - Mathf.Atan(1f / 2f);
            longitude += angle2;
            Vector3 v6 = SphericalCoord(longitude, latitude, radius);

            vertices.Add(v1);
            vertices.Add(v3);
            vertices.Add(v2);
            vertices.Add(v4);
            vertices.Add(v5);
            vertices.Add(v6);
        }

        for (int i = 0; i < subdivisions; ++i)
            vertices = Subdivise(vertices);

        return vertices;
    }

    public static List<Vector3> Subdivise(List<Vector3> list)
    {
        List<Vector3> otherList = new List<Vector3>();

        float radius = list[0].magnitude;

        for (int i = 0; i < list.Count / 3; ++i)
        {
            Vector3 v1 = list[3 * i + 0];
            Vector3 v2 = list[3 * i + 1];
            Vector3 v3 = list[3 * i + 2];

            Vector3 v12 = radius * (v1 + v2).normalized;
            Vector3 v23 = radius * (v2 + v3).normalized;
            Vector3 v31 = radius * (v3 + v1).normalized;

            // center
            otherList.Add(v12);
            otherList.Add(v23);
            otherList.Add(v31);

            otherList.Add(v1);
            otherList.Add(v12);
            otherList.Add(v31);

            otherList.Add(v2);
            otherList.Add(v23);
            otherList.Add(v12);

            otherList.Add(v3);
            otherList.Add(v31);
            otherList.Add(v23);
        }


        return otherList;
    }

    public static Vector3 SphericalCoord(float longitude, float latitude, float radius)
    {
        return radius * new Vector3(
            Mathf.Sin(latitude) * Mathf.Sin(longitude),
            Mathf.Cos(latitude),
            Mathf.Sin(latitude) * Mathf.Cos(longitude)
            );
    }
}
