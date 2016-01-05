using UnityEngine;
using System.Collections;

public class CameraPos : MonoBehaviour {

	private Animator anim;

	void Awake() {
		anim = transform.parent.GetComponentInChildren<Animator>();
	}

	void Start() {
		transform.parent = anim.GetBoneTransform(HumanBodyBones.Head).transform; 
		transform.localPosition = new Vector3(0.0f, 0.08f, 0.15f);
		transform.localEulerAngles = new Vector3(0f, 0f, 6.0f);
	}
}
