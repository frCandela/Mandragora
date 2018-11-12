using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MagicWand : VRTK_InteractableObject
{
	[SerializeField] TrailRenderer m_trail;
	[SerializeField] AngularVelocityTracker m_tip;
	GestureHandler m_gestureHandler;

	bool IsUsed
	{
		set{
			m_trail.emitting = value;
		}
	}

	public override void StartUsing(VRTK_InteractUse usingObject)
	{
		base.StartUsing(usingObject);

		IsUsed = true;
	}

	public override void StopUsing(VRTK_InteractUse previousUsingObject)
	{
		base.StopUsing(previousUsingObject);

		IsUsed = false;
	}
}
