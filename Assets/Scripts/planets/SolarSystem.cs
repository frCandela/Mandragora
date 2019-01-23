using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] public Effect effectPrefab = null;
    [SerializeField] public List<AK.Wwise.State> m_states = new List<AK.Wwise.State>();
    List<PlanetEffect> m_planetEffectsList = new List<PlanetEffect>();

    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.attachedRigidbody)
        {
            
            MTK_Interactable interactable = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interactable && interactable.isGrabbable && !interactable.isDistanceGrabbed)
            {
                
                PlanetEffect eff = (PlanetEffect)effectPrefab.AddEffectTo(other.attachedRigidbody.gameObject);
                if (eff)
                {
                    eff.sunRigidbody = m_rb;
                    m_planetEffectsList.Add(eff);

                    UpdateState(m_planetEffectsList.Count);
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        PlanetEffect effect = other.attachedRigidbody.GetComponent<PlanetEffect>();
        if (effect)
        {
            m_planetEffectsList.Remove(effect);
            Destroy(effect);

            UpdateState(m_planetEffectsList.Count);
        }
    }

    void UpdateState(int count)
    {
        if(count >= 0 && count < m_states.Count)
        {
            AkSoundEngine.SetState(m_states[count].GroupId, m_states[count].Id);
        }
    }
}