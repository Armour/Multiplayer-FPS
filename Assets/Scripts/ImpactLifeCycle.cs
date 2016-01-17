using UnityEngine;
using System.Collections;

public class ImpactLifeCycle : Photon.MonoBehaviour {
	
	void Start() {
		GetComponent<ParticleSystem>().Play();
		Destroy(gameObject, 1.5f);
	}
}
