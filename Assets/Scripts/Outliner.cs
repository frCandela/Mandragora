using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outliner : MonoBehaviour
{
    [SerializeField] Outline.Mode m_mode = Outline.Mode.OutlineAll;
    [SerializeField] Color m_color = Color.red;
    [SerializeField] float m_outlineWidth = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if( other.GetComponent<MTK_Interactable>())
        {           
            if ( ! other.GetComponent<Outline>())
            {
                Outline outline = other.gameObject.AddComponent<Outline>();
                outline.OutlineMode = m_mode;
                outline.OutlineColor = m_color;
                outline.OutlineWidth = m_outlineWidth;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Outline outline = other.GetComponent<Outline>();
        if (outline)
        {
            Destroy(outline);
        }
    }
}
