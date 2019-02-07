using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public abstract class Workshop : MonoBehaviour
{
	protected DropZone m_dropzone;

	void Awake ()
    {
        m_dropzone = GetComponent<DropZone>();
        m_dropzone.onObjectCatched.AddListener(EnableWorkshop);
    }

    protected void EnableWorkshop(bool state)
	{
		MTK_Interactable current = m_dropzone.catchedObject;

		if(state)
		{
			current.isDistanceGrabbable = false;
            current.IndexJointUsed = 1;
		}
		else
		{
			current.IndexJointUsed = 0;
            current.isDistanceGrabbable = true;
		}

		OnWorkshopUpdateState(state, current);
	}

	protected abstract void OnWorkshopUpdateState(bool state, MTK_Interactable current);
}
