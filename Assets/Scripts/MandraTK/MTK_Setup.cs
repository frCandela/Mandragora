using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public abstract class MTK_Setup : MonoBehaviour
{
    public GameObject leftHand = null;
    public GameObject rightHand = null;
    public GameObject head = null;

    [SerializeField] DropZone m_scalerDrop, m_tileDrop, m_placerDrop;

    List<MTK_Interactable> m_grabbedList = new List<MTK_Interactable>();

    public abstract void UpdateSettings();
    
    public void NotifyGrab(MTK_Interactable grabbed)
    {
        m_grabbedList.Add(grabbed);

        m_scalerDrop.enabled = true;

        if(grabbed.GetComponent<Icosphere>())
            m_tileDrop.enabled = m_placerDrop.enabled = true;
    }

    public void NotifyRelease(MTK_Interactable released)
    {
        m_grabbedList.Remove(released);

        if(m_grabbedList.Count == 0)
        {
            m_scalerDrop.enabled = false;
            m_tileDrop.enabled = m_placerDrop.enabled = false;
        }
    }

    protected void CheckSetup()
    {
        Util.EditorAssert(head != null, "Please select a head gameobject");
        Util.EditorAssert(leftHand != null, "Please select a leftHand gameobject");
        Util.EditorAssert(rightHand != null, "Please select a rightHand gameobject");
    }
}
