using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameExporter : MonoBehaviour
{
    public string nameMesh = "default";
    public bool once = false;

    public MeshFilter meshFilter;

    // Update is called once per frame
    void Update()
    {
        if (once)
        {
            once = false;

            ObjExporter.MeshToFile(meshFilter, "Assets/" + nameMesh + ".obj");
            print("Mesh successfully exported : " + nameMesh + ".obj");
        }
    }
}