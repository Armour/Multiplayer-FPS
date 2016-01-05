using UnityEngine;
using System.Collections;

public class WeaponPos : MonoBehaviour {

	public float mouseSensitivity = 0.5f;

	private Animator anim;
	private float maxRotation = 293f;
	private float minRotation = 272f;
	private float rotationY = 0;

	void Awake() {
		anim = transform.parent.GetComponentInChildren<Animator>();
		//rotationY = (maxRotation + minRotation) / 2;
	}

	void Start() {
		transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand).transform; 
		transform.localPosition = new Vector3(0.016f, 0.34f, 0.006f);
		transform.localEulerAngles = new Vector3(287f, 110f, 348f);
	}

	void FixedUpdate() {
		rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		rotationY = Mathf.Clamp(rotationY, minRotation - 287f, maxRotation - 287f);
		transform.localEulerAngles = new Vector3(rotationY + 287f, 110f, 348f);
	}
}
