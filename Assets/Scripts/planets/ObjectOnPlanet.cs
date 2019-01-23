using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOnPlanet : MonoBehaviour
{
    public GameObject referenceObject;

    public void Dissociate()
    {
        referenceObject.transform.position = transform.position;
        referenceObject.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(gameObject);
    }
}
