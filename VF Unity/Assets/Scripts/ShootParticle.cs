using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootParticle : MonoBehaviour {

	public ParticleSystem shootParticles1;
	public ParticleSystem shootParticles2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetButtonDown ("Fire1"))
			shootParticles1.Emit (1);
		if (Input.GetButtonDown ("Fire2"))
			shootParticles2.Emit (1);
		
	}
}
