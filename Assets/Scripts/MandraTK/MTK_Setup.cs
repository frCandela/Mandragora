using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_Setup : MonoBehaviour
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    // Use this for initialization
    void Awake ()
    {
        if (!leftHand)
            leftHand = new GameObject("leftHand");
        if (!rightHand)
            rightHand = new GameObject("rightHand");
        if (!head)
            head = new GameObject("head");


        leftHand.transform.parent = transform;
        rightHand.transform.parent = transform;
        head.transform.parent = transform;
        Camera.main.transform.parent = head.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
