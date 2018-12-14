using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Outliner : MonoBehaviour
{
    [SerializeField] Outline.Mode m_mode = Outline.Mode.OutlineAll;
    [SerializeField] Color m_color = Color.red;
    [SerializeField] float m_outlineWidth = 10f;

    public void OultineOn(MTK_Interactable interractable)
    {
        if ( ! interractable.GetComponent<Outline>())
        {
            Outline outline = interractable.gameObject.AddComponent<Outline>();
        }
    }

    public void OultineOff(MTK_Interactable interractable)
    {
        Outline outline = interractable.GetComponent<Outline>();

        if (outline)
            Destroy(outline);
    }
}
