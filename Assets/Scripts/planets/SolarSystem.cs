using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class SolarSystem : MonoBehaviour
{
    [SerializeField] private float m_maxSpeed = float.PositiveInfinity;
    [SerializeField] private float m_accelerationForce = 1;
    [SerializeField] private float impactForce = 3f;
    [SerializeField] public List<AK.Wwise.State> m_states = new List<AK.Wwise.State>();

    [SerializeField] SmartTrigger m_exclusionPlatform;
    [SerializeField] SmartTrigger m_exclusionCloseRadius;

    private Rigidbody m_rb;

    [SerializeField] GameObject m_explosionEffect;
    [SerializeField] MTK_TPZone_Planet m_planetTPZone;

    bool m_canTPPlanet = false;

    public IcoPlanet lastPlanet
    {
        get
        {
            for (int i = m_planetList.Count - 1; i >= 0; --i)
            {
                IcoPlanet planet = m_planetList[m_planetList.Count - 1];
                if (!m_exclusionPlatform.m_objectsInTrigger.Contains(planet.gameObject))
                {
                    return planet;
                }
            }
            return null;
        }
    }

    List<PlanetEffect> m_planetEffectsList = new List<PlanetEffect>();
    List<IcoPlanet> m_planetList = new List<IcoPlanet>();

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();

        Assert.IsNotNull(m_exclusionPlatform);
        Assert.IsNotNull(m_exclusionCloseRadius);

        m_exclusionCloseRadius.onEnter.AddListener(onEnterCloseRadius);
        m_exclusionCloseRadius.onExit.AddListener(onExitCloseRadius);

        m_exclusionPlatform.onEnter.AddListener(onEnterExclusionZone);
        m_exclusionPlatform.onExit.AddListener(onExitExclusionZone);
    }


    void onEnterCloseRadius(GameObject obj)
    {
        PlanetEffect effect = obj.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = false;
        }
    }

    void onExitCloseRadius( GameObject obj)
    {
        PlanetEffect effect = obj.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = true;
        }
    }

    void onEnterExclusionZone(GameObject obj)
    {
        PlanetEffect effect = obj.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = false;
            effect.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void onExitExclusionZone(GameObject obj)
    {
        PlanetEffect effect = obj.GetComponent<PlanetEffect>();
        if (effect)
        {
            effect.effectActive = true;
            effect.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody)
        {            
            MTK_Interactable interactable = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interactable && interactable.isGrabbable && !interactable.isDistanceGrabbed)
            {
                UpdateTPZone(interactable.GetComponent<IcoPlanet>(), true);
                
                if( !other.attachedRigidbody.gameObject.GetComponent< PlanetEffect>())
                {
                    if(!other.GetComponentInParent<IcoSegment>())
                    {
                        PlanetEffect eff = other.attachedRigidbody.gameObject.AddComponent<PlanetEffect>();
                        if (eff)
                        {
                            eff.maxSpeed = m_maxSpeed;
                            eff.accelerationForce = m_accelerationForce;
                            eff.impactForce = impactForce;
                            eff.sunRigidbody = m_rb;
                            m_planetEffectsList.Add(eff);

                            eff.explosionEffect = m_explosionEffect;

                            UpdateState(m_planetEffectsList.Count);

                            if( m_exclusionPlatform.m_objectsInTrigger.Contains( eff.gameObject ))
                            {
                                eff.effectActive = false;
                                eff.GetComponent<Rigidbody>().useGravity = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody)
        {
            PlanetEffect effect = other.attachedRigidbody.GetComponent<PlanetEffect>();
            UpdateTPZone(other.GetComponent<IcoPlanet>(), false);

            if (effect)
            {
                m_planetEffectsList.Remove(effect);
                Destroy(effect);

                UpdateState(m_planetEffectsList.Count);
            }
        }
    }

    void UpdateTPZone(IcoPlanet planet, bool enter)
    {
        if(m_canTPPlanet)
        {
            if(planet)
            {
                if(enter)
                {
                    if( ! m_planetList.Contains(planet) )
                    {
                        m_planetList.Add(planet);
                    }
                }
                else
                {
                    m_planetList.Remove(planet);
                }   
            }

            m_planetTPZone.Planet = lastPlanet;
        }
    }

    public void EnablePlanetTP()
    {
        m_canTPPlanet = true;
    }

    void UpdateState(int count)
    {
        if(count >= 0 && count < m_states.Count)
        {
            AkSoundEngine.SetState(m_states[count].GroupId, m_states[count].Id);
        }
    }
}