using UnityEngine;
using System.Collections;

public class WeaponPos : MonoBehaviour {

	private Animator anim;

	void Awake() {
		anim = transform.parent.GetComponentInChildren<Animator>();
	}

	void Start() {
		transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand).transform; 
		transform.localPosition = new Vector3(0.03f, 0.34f, 0.003f);
		transform.localEulerAngles = new Vector3(290f, 120f, 0f);
	}
}
