using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SunCloseRadius : MonoBehaviour
{
    [SerializeField] private bool disableGravity = false;

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
            if (disableGravity)
            {
                effect.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        IcoPlanet planet = other.attachedRigidbody.GetComponent<IcoPlanet>();
        if (planet)
        {
            m_solarSystem.SetPlanetOutOfZone(planet, true);
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

        IcoPlanet planet = other.attachedRigidbody.GetComponent<IcoPlanet>();
        if (planet)
        {
            m_solarSystem.SetPlanetOutOfZone(planet, false);
        }
    }

}
