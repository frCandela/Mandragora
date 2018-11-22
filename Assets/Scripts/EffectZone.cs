using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour
{
    [SerializeField] public Effect effectPrefab = null;

    private void OnTriggerEnter(Collider other)
    {
        LevitationEffect levitating = other.GetComponent<LevitationEffect>();
        if( ! levitating )
        {
            effectPrefab.AddEffectTo(other.gameObject);
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
