using UnityEngine;
using System.Collections;

public class ImpactLifeCycle : Photon.MonoBehaviour {

    // Called when game start
    void Start() {
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1.5f);
    }

}
