using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SphereCollider))]
public class SunCloseRadius : MonoBehaviour
{
    [SerializeField] private bool disableGravity = false;

    private void Awake()
    {
        Assert.IsTrue(GetComponent<SphereCollider>().isTrigger, "SphereCollider must be a trigger");
    }

    private void OnTriggerStay(Collider other)
    {
        PlanetEffect effect = other.GetComponent<PlanetEffect>();
        if( effect)
        {
            effect.effectActive = false;
            if(disableGravity)
            {
                effect.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlanetEffect effect = other.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = true;
            effect.GetComponent<Rigidbody>().useGravity = false;
        }
    }

}
