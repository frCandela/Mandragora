using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPortal : Spawner
{
    MTK_InputManager m_inputHand;
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
        if(other.GetComponent<MTK_InputManager>())
        {
            m_inputHand = other.GetComponent<MTK_InputManager>();

            m_correctEntry = GetTriggerNormal(other.transform.position) == Vector3.up;
            m_canSpawn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_inputHand == other.GetComponent<MTK_InputManager>())
            if(m_correctEntry)
                if(GetTriggerNormal(other.transform.position) == Vector3.down)
                {
                    // vibration
                    m_canSpawn = true;
                }
        // else
        //     print("failure");
    }

    public void Cancel()
    {
        m_correctEntry = false;
        m_canSpawn = false;
    }

    private void Update()
    {
        if(m_canSpawn)
            m_inputHand.Haptic(Time.deltaTime);
    }
}
