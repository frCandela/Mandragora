using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_Interactable : MonoBehaviour
{
    [SerializeField] public bool isGrabbable = true;
    [SerializeField] public MTK_JointType jointType = null;

    // Use this for initialization
    void Awake ()
    {
        if (!jointType)
            jointType = gameObject.AddComponent<MTK_JointType_Fixed>();

    }
}
