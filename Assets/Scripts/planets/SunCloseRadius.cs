using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SunCloseRadius : MonoBehaviour
{
    private SolarSystem m_solarSystem;

    private void Awake()
    {
        m_solarSystem = FindObjectOfType<SolarSystem>();

        Assert.IsNotNull(m_solarSystem, "No solarSystem found");
        Assert.IsTrue(GetComponent<Collider>().isTrigger, "Collider must be a trigger");
    }

    private void OnTriggerStay(Collider other)
    {
        PlanetEffect effect = other.attachedRigidbody.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = false;
        }
    }    



    private void OnTriggerExit(Collider other)
    {
        PlanetEffect effect = other.attachedRigidbody.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = true;
        }
    }

}
