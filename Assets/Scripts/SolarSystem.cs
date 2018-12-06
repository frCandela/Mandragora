using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] public Effect effectPrefab = null;

    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlanetEffect eff = (PlanetEffect)effectPrefab.AddEffectTo(other.gameObject);
        if (eff)
        {
            eff.sunRigidbody = m_rb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Effect effect = (Effect)other.GetComponent(effectPrefab.GetType());
        if (effect)
        {
            Destroy(effect);
        }
    }

}
