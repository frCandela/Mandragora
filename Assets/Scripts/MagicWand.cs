using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MagicWand : VRTK_InteractableObject
{
	[SerializeField] TrailRenderer m_trail;
	[SerializeField] AngularVelocityTracker m_tip;
	[SerializeField] GestureHandler m_gestureHandler;

	bool IsUsed
	{
		set{
			m_trail.emitting = value;
			m_gestureHandler.Collecting = value;
		}
	}

	public override void Grabbed(VRTK_InteractGrab currentGrabbingObject)
	{
		base.Grabbed(currentGrabbingObject);

		m_gestureHandler.InitTracker(m_tip);
		m_gestureHandler.AddGesture();
	}

	public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
	{
		base.Ungrabbed(previousGrabbingObject);
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
