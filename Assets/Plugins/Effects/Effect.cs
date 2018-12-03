using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    [SerializeField] private GameObject m_particlesPrefab = null;
    private GameObject m_particles = null;

    private void OnDestroy()
    {
        RemoveEffect();
        if(m_particles)
            Destroy(m_particles.gameObject);
    }

    // Called with a prefab so customize particles and stuff
    public virtual bool AddEffectTo( GameObject target )
    { 
        if( ! target.GetComponent( GetType()))
        {
            Effect effect = (Effect)target.AddComponent(GetType());
            if (effect.ApplyEffect())
            {
                // Adds particles
                if (m_particlesPrefab)
                {
                    effect.m_particles = Instantiate(m_particlesPrefab, target.transform);
                    effect.m_particles.transform.localRotation = Quaternion.identity;
                    effect.m_particles.transform.localPosition = Vector3.zero;
                }
                return true;
            }
        }
        return false;
    }

    public abstract bool ApplyEffect();    
    public abstract void RemoveEffect();
}

