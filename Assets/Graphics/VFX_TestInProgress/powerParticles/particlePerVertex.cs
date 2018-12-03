using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class GoEvent : UnityEvent<GameObject> {}

public class particlePerVertex : MonoBehaviour {

	public GameObject sourceMesh; // JUST FOR DEMO
	public int maxParticlesNumber;
	public float particleSize;
	public Vector2 minMaxLerp = new Vector2(0.05f, 0.15f);
	[Range(0.0f, 2.0f)] public float bVelOffset = 1.44f;
	[Range(0f,0.5f)]public float bSlowFactor = 0.25f;
	public float bMaxVelocity = 1000.0f;
	public float bMaxDistFromTarget = 10.0f;
	public float bToTargetFactor = 6.0f;
	public float bNoiseStrength = 1.0f;
	public Vector2 bMinMaxDistNoiseThreshold = new Vector2(5.0f, 30.0f);
	public Vector2 bMinMaxNoiseAmp = new Vector2(2.0f, 3.0f);
	public Vector2 bMinMaxNoiseFreq = new Vector2(1.0f, 3.0f);

	public GoEvent initEvent;
	private ParticleSystem PS;
	private ParticleSystem.Particle[] particles;
	private Mesh mesh;
	private float[] bRandomOffsets;
	private Vector3[] wVertices;
	private float[] lerpFactors;
	private float[] distFromTarget;
	private float[] noiseAmp;
	private float[] noiseFreq;

	// Use this for initialization
	void Start () {

		// Setup Event
		initEvent.AddListener(InitializePS);

		// Setup Particle System
		PS = this.GetComponent<ParticleSystem>();
		var main = PS.main;
		main.startSpeedMultiplier = 0.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.startLifetime = 1.0f;
		main.startSpeed = 0.0f;
		main.startSize = particleSize;
		var emission = PS.emission;
		emission.enabled = false;
		var shape = PS.shape;
		shape.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)) initEvent.Invoke(sourceMesh); // JUST FOR DEMO
		
		
		int particleCount = PS.particleCount;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
		PS.GetParticles(particles);
		
		if(particleCount <= 0) return; // no calculation if no particles

		// Here is Particles Calculation
		for(int i=0; i<particleCount; i++)
		{
			particles[i].remainingLifetime = 1.0f;
			Vector3 vertexWorldPos = transform.TransformPoint(mesh.vertices[i]);
			Vector3 pPos = particles[i].position;
			// particles[i].position = Vector3.Lerp(pPos, vertexWorldPos, lerpFactors[i]); // lerpBased
			Vector3 pVel = particles[i].velocity;
			Vector3 newVel = ProcessParticleVelocity(pVel, pPos, vertexWorldPos, i); // Boid based position
			particles[i].velocity = newVel;											// Boid based position

			// NOISE offset position
			particles[i].position = ApplyNoise(pPos, distFromTarget[i], bNoiseStrength, i);
		}

		PS.SetParticles(particles, particleCount);
		
	}

	public void InitializePS (GameObject source) {
		// Parent the effect to the target GameObject
		this.transform.parent = sourceMesh.transform;
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;

		// Setup mesh and wVertices[]
		mesh = source.GetComponent<MeshFilter>().mesh;
		source.GetComponent<MeshRenderer>().enabled = false;

		int nbParticles = mesh.vertices.Length; // variables different for each particle

		if(nbParticles > maxParticlesNumber) Debug.LogError("Too much Vertices, only " + maxParticlesNumber + " particles created.");
		nbParticles = Mathf.Clamp(nbParticles, 0, maxParticlesNumber);
		//lerpFactors = new float[nbVertices]; // Lerp based
		bRandomOffsets = new float[nbParticles];
		distFromTarget = new float[nbParticles];
		noiseFreq = new float[nbParticles];
		noiseAmp = new float[nbParticles];

		// Creates particles
		if(nbParticles >= 50) Debug.LogError(nbParticles + " verts for new " + source.name + ". CPU said  :'(");
		else print(nbParticles + " verts for new " + source.name);

		PS.Emit(nbParticles);


		// Setup just created particles
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[nbParticles];
		PS.GetParticles(particles);

		for(int i=0; i < nbParticles; i++)
		{
			//lerpFactors[i] = Random.Range(minMaxLerp.x, minMaxLerp.y); // Lerp Based
			bRandomOffsets[i] = Random.Range(1 - (bVelOffset/2), 1 + (bVelOffset/2));
			noiseAmp[i] = Random.Range(bMinMaxNoiseAmp.x, bMinMaxNoiseAmp.y);
			noiseFreq[i] = Random.Range(bMinMaxNoiseFreq.x, bMinMaxNoiseFreq.y);
			particles[i].position = transform.TransformPoint(mesh.vertices[i]);
		}

		PS.SetParticles(particles, nbParticles);
		
		
	}

	public Vector3 ApplyNoise (Vector3 position, float distFromT, float noiseFactor,  int i) {
		Vector3 noise = Vector3.zero;

		noise.x = noiseAmp[i] * Mathf.Sin(noiseFreq[i] * position.y) + noiseAmp[i] * Mathf.Cos(noiseFreq[i] * position.z);
		noise.y = noiseAmp[i] * Mathf.Sin(noiseFreq[i] * position.z) + noiseAmp[i] * Mathf.Cos(noiseFreq[i] * position.x);
		noise.z = noiseAmp[i] * Mathf.Sin(noiseFreq[i] * position.x) + noiseAmp[i] * Mathf.Cos(noiseFreq[i] * position.y);

		float distFactor = (distFromT - bMinMaxDistNoiseThreshold.x) / (bMinMaxDistNoiseThreshold.y - bMinMaxDistNoiseThreshold.x);
		distFactor = Mathf.Clamp(distFactor, 0, 1);

		noise *= noiseFactor * distFactor * 0.1f;

		return position + noise;
	}

	private Vector3 ProcessParticleVelocity(Vector3 currentVel, Vector3 currentPos, Vector3 targetPos, int i) {

		Vector3 addVel = Vector3.zero;

		// Go to target
		Vector3 toTargetVector = (targetPos - currentPos);
		toTargetVector *= bToTargetFactor;

		// Store distance from target
		distFromTarget[i] = toTargetVector.magnitude;

		// Is the particle going away
		Vector3 futurDist = targetPos - (currentVel + currentPos);

		float velSqrMag = currentVel.sqrMagnitude;
		if(velSqrMag > 0.0f && futurDist.sqrMagnitude >= toTargetVector.sqrMagnitude)
		{
			addVel = -currentVel * bSlowFactor;
		}

		// Apply
		addVel += toTargetVector;
		addVel *= bRandomOffsets[i];

		Vector3 newVel = addVel + currentVel;

		//Limit Velocity
		if(newVel.sqrMagnitude > bMaxVelocity * bMaxVelocity)
		{
			newVel = Vector3.Normalize(newVel) * bMaxVelocity;
		}

		//print(addVel.magnitude);
		return newVel;
	}
}
