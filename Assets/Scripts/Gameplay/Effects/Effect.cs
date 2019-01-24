using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    [SerializeField] private GameObject m_particlesPrefab = null;
    public bool affectsRigidbody = true;

    private GameObject m_particles = null;

    private void OnDestroy()
    {
        RemoveEffect();
        if(m_particles)
            Destroy(m_particles.gameObject);
    }

    // Called with a prefab so customize particles
    public virtual Effect AddEffectTo( GameObject target )
    { 
        if( ! target.GetComponent(GetType())) // no duplication
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
                return effect;
            }
        }
        return null;
    }

    private void OnEnable()
    {
        ApplyEffect();
    }

    private void OnDisable()
    {
        RemoveEffect();
    }

    public abstract bool ApplyEffect();    
    public abstract void RemoveEffect();
}

