using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPortal : Spawner
{
    MTK_InteractHand m_inputHand;
    bool m_correctEntry = false;
    bool m_canSpawn = false;

    Vector3 GetTriggerNormal(Vector3 origin)
    {
        Vector3 normal = default(Vector3);
        RaycastHit hit;
        
        if (Physics.Raycast (new Ray (origin, transform.position - origin), out hit, 1))
            normal = Quaternion.Inverse(transform.rotation) * hit.normal;

        return normal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MTK_InteractHand>())
        {
            m_inputHand = other.GetComponent<MTK_InteractHand>();

            m_correctEntry = GetTriggerNormal(other.transform.position) == Vector3.up;
            m_canSpawn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_inputHand == other.GetComponent<MTK_InteractHand>())
            if(m_correctEntry)
                if(GetTriggerNormal(other.transform.position) == Vector3.down)
                {
                    // vibration
                    m_canSpawn = true;
                }
        // else
        //     print("failure");
    }

    public void AttemptGrab()
    {
        if(m_canSpawn)
        {
            Spawn(m_inputHand.transform.position, m_inputHand.transform.rotation, null);
            m_inputHand.Grab(m_spawned.GetComponent<MTK_Interactable>());
            m_canSpawn = false;
        }
    }

    public void Cancel()
    {
        m_correctEntry = false;
        m_canSpawn = false;
    }

    private void Update()
    {
        if(m_canSpawn)
            m_inputHand.GetComponent<MTK_InputManagerSimulator>().Haptic(Time.deltaTime);
    }
}
