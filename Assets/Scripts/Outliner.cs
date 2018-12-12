using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Outliner : MonoBehaviour
{
    MTK_Interactable m_outlinedObject;
    MTK_Interactable OutlinedObject
    {
        set
        {
            if(m_outlinedObject != value)
            {
                // Off old
                if(m_outlinedObject)
                {
                    foreach (var renderer in m_outlinedObject.GetComponentsInChildren<Renderer>())
                    {
                        //weird bug here
                        if (!renderer)
                            continue;

                        // Remove outline shaders
                        var materials = renderer.sharedMaterials.ToList();

                        materials.Remove(m_material);

                        renderer.materials = materials.ToArray();
                    }
                }  
                
                // On New
                if(value)
                {
                    foreach (var renderer in value.GetComponentsInChildren<Renderer>())
                    {
                        // Append outline shaders
                        var materials = renderer.sharedMaterials.ToList();

                        materials.Add(m_material);

                        renderer.materials = materials.ToArray();
                    }
                }

                m_outlinedObject = value;
            }
            
        }
    }

    [SerializeField] Material m_material;

    public void OultineOn(MTK_Interactable interractable)
    {
        OutlinedObject = interractable;
    }

    public void OultineOff(MTK_Interactable interractable)
    {
        OutlinedObject = null;
    }
}
