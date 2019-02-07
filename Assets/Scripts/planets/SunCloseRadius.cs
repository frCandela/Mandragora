using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SphereCollider))]
public class SunCloseRadius : MonoBehaviour
{
    private void Awake()
    {
        Assert.IsTrue(GetComponent<SphereCollider>().isTrigger, "SphereCollider must be a trigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlanetEffect effect = other.GetComponent<PlanetEffect>();
        if( effect)
        {
            effect.effectActive = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlanetEffect effect = other.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = true;
        }
    }

}
