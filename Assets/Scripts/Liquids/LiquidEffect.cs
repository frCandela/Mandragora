using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LiquidEffect : MonoBehaviour
{
    public abstract void ApplyEffect( GameObject gameObject);
    public abstract void RemoveEffect(GameObject gameObject);
}
