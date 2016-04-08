using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyToggleEnabled : MonoBehaviour {
  public List<GameObject> targets;
  public KeyCode toggle = KeyCode.T;

	// Update is called once per frame
	void Update () {
	  if (Input.GetKeyDown (toggle)) {
      foreach (GameObject target in targets) {
        target.SetActive(!target.activeSelf);
      }
    }
	}
}
