using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DatePickerHideVolume : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other) {
    Text text = other.GetComponentInChildren<Text> ();
    if (text == null) { return; }
    text.enabled = false;
	}
	
	void OnTriggerExit(Collider other) {
    Text text = other.GetComponentInChildren<Text> ();
    if (text == null) { return; }
    text.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
