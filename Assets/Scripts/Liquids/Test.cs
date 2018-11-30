using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    [Range(0,10)]public float flow = 1f;
    Obi.ObiEmitter emmiter;

	// Use this for initialization
	void Start ()
    {
        emmiter = GetComponent<Obi.ObiEmitter>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        emmiter.unemittedBursts += flow;

    }
}
