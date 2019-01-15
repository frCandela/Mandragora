using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Outliner : MonoBehaviour
{
    public static void OultineOn(MTK_Interactable interractable)
    {
        if ( ! interractable.GetComponent<Outline>())
        {
            interractable.gameObject.AddComponent<Outline>();
        }
    }

    public static void OultineOff(MTK_Interactable interractable)
    {
        Outline outline = interractable.GetComponent<Outline>();

        if (outline)
            Destroy(outline);
    }
}
