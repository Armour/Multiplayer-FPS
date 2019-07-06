using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]

public class ImpactLifeCycle : MonoBehaviour {

    [SerializeField]
    private float lifespan = 1.5f;

    private ParticleSystem particleEffect;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, lifespan);
    }

}
