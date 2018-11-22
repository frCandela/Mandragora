using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_Interactable : MonoBehaviour
{
    [SerializeField] public bool isGrabbable = true;
    [HideInInspector] public MTK_JointType jointType = null;

    // Use this for initialization
    void Awake ()
    {
        if (isGrabbable && !jointType)
            jointType = gameObject.AddComponent<MTK_JointType_Fixed>();

    }
}
