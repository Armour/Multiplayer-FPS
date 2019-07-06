using UnityEngine;

[RequireComponent(typeof(Animator))]

public class DoorAnimation : MonoBehaviour {

    [SerializeField]
    private float minPosY = 2.74f;
    [SerializeField]
    private float maxPosY = 5.9f;

    private Animator animator;
    private float posx;
    private float posz;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        animator = GetComponent<Animator>();
        posx = transform.position.x;
        posz = transform.position.z;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        transform.position = new Vector3(posx, Mathf.Clamp(transform.position.y, minPosY, maxPosY), posz);
    }

    /// <summary>
    /// OnTriggerStay is called once per frame for every Collider other
    /// that is touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            animator.SetBool("Trigger", true);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            animator.SetBool("Trigger", false);
        }
    }

}
