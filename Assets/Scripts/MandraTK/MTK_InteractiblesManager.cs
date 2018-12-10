using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_InteractiblesManager : Singleton<MTK_InteractiblesManager>
{
	List<MTK_Interactable> m_grabbableesList = new List<MTK_Interactable>();

	public void Subscribe(MTK_Interactable input)
	{
		m_grabbableesList.Add(input);
	}

	public MTK_Interactable GetClosestToView(Transform pointer, float maxAngle)
	{
		float minAngle = maxAngle;
		MTK_Interactable closest = null;
		float angle;

		Vector3 pointerPos = pointer.position,
				pointerForward = pointer.forward;;

		foreach (MTK_Interactable item in m_grabbableesList)
		{
			angle = Vector3.Angle(item.transform.position - pointerPos, pointerForward);

			if(angle < minAngle)
			{
				closest = item;
				minAngle = angle;
			}
		}

		return closest;
	}
}
