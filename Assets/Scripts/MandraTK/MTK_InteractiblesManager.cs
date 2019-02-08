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

	public void UnSubscribe(MTK_Interactable input)
	{
		m_grabbableesList.Remove(input);
	}

	public MTK_Interactable GetClosestToView(Vector3 position, Vector3 direction, float maxAngle)
	{
		float minAngle = maxAngle;
		MTK_Interactable closest = null;
		float angle;

		foreach (MTK_Interactable item in m_grabbableesList)
		{
			if(item.isDistanceGrabbable)
			{
				angle = Vector3.Angle(item.transform.position - position, direction);

				if(angle < minAngle)
				{
					closest = item;
					minAngle = angle;
				}
			}
		}

		return closest;
	}
}
