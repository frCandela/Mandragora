using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_SolidHand : MonoBehaviour
{
    private MTK_InteractHand m_interactHand;
    private TelekinesisPointer m_telekinesisPointer;
    private Collider m_collider;

    private void Awake()
    {
        m_interactHand = GetComponent<MTK_InteractHand>();
        m_collider = GetComponent<Collider>();
        m_telekinesisPointer = GetComponent<TelekinesisPointer>();
    }

    private void Update()
    {
        if ( ! m_telekinesisPointer.isAttracting && m_interactHand.m_grabPressed && ! m_interactHand.m_grabbed)
        {
            m_collider.isTrigger = false;
        }
        else
        {
            m_collider.isTrigger = true;
        }
    }
}
