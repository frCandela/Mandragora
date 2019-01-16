using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpVfxInstanciate : MonoBehaviour {

	public ParticleSystem psIn;
	public ParticleSystem psOut;
	[Range(0.0f, 0.05f)] public float psInDepopDelay;
	public float TpChargeDuration;
	private float launchTimer;
	private bool isLaunch;

	// Use this for initialization
	void Start () {
		var mainIn = psIn.main;
		mainIn.startLifetime = TpChargeDuration + psInDepopDelay; // Add 0.02 to prevent blank frame between 2 PS
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)) LaunchInTpVfx();


		if(isLaunch && launchTimer + TpChargeDuration <= Time.time)
		{
			LaunchOutTpVfx();
			isLaunch = false;
		}
        
	}

	public void LaunchInTpVfx () {
		launchTimer = Time.time;
		isLaunch = true;
		psIn.Emit(3500);
	}

	public void LaunchOutTpVfx () {
		// /////////////////////////////////////////// TP NOW
		psOut.Emit(3500);
	}
}
