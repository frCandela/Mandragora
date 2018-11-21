using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationZone : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        LevitationEffect levitating = other.GetComponent<LevitationEffect>();
        if( ! levitating )
        {
            other.gameObject.AddComponent<LevitationEffect>();
            print("zi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LevitationEffect levitating = other.GetComponent<LevitationEffect>();
        if (levitating)
        {
            Destroy(levitating);
            print("zout");
        }
    }
}
