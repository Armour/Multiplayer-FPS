using UnityEngine;
using System.Collections;

public class DoorAnimation : MonoBehaviour {

    private Animator anim;
    private float stx;
    private float stz;

    // Called when script awake in editor
    void Awake() {
        stx = transform.position.x;
        stz = transform.position.z;
    }

    // Called when game start
    void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(stx, Mathf.Clamp(transform.position.y, 2.74f, 5.9f), stz);
    }

    // When player stay in the strigger area, let the door open
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player")
            anim.SetBool("Trigger", true);
    }

    // When player exit the strigger area, let the door close
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player")
            anim.SetBool("Trigger", false);
    }

}
