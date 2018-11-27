using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public abstract class MTK_Setup : MonoBehaviour
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    public abstract void UpdateSettings();

    protected void CheckSetup()
    {
        Util.EditorAssert(head != null, "Please select a head gameobject");
        Util.EditorAssert(leftHand != null, "Please select a leftHand gameobject");
        Util.EditorAssert(rightHand != null, "Please select a rightHand gameobject");
    }
}
