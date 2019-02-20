using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTrigger : MonoBehaviour
{
    public List<GameObject> m_objectsInTrigger;

    public UnityEventGameObject onEnter;
    public UnityEventGameObject onExit;

    private void OnTriggerEnter(Collider other)
    {
        if( !m_objectsInTrigger.Contains(other.attachedRigidbody.gameObject))
        {
            m_objectsInTrigger.Add(other.attachedRigidbody.gameObject);
            onEnter.Invoke(other.attachedRigidbody.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        m_objectsInTrigger.Remove(other.attachedRigidbody.gameObject);
        onExit.Invoke(other.attachedRigidbody.gameObject);
    }
}
