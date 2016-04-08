using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeapImageBasedMaterial : MonoBehaviour {
    public ImageMode imageMode = ImageMode.STEREO;

    public enum ImageMode {
        STEREO,
        LEFT_ONLY,
        RIGHT_ONLY
    }

    void Awake() {
        if (FindObjectOfType<LeapImageRetriever>() == null) {
            Debug.LogWarning("Place a LeapImageRetriever script on a camera to enable Leap image-based materials");
            enabled = false;
        }
    }

    void OnEnable() {
        LeapImageRetriever.registerImageBasedMaterial(this);
        // Make shader consistent with settings
        
        if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
          GetComponent<Renderer> ().material.SetFloat ("_ColorSpaceGamma", 1.0f);
        } else {
          float gamma = -Mathf.Log10(Mathf.GammaToLinearSpace(0.1f));
          GetComponent<Renderer> ().material.SetFloat ("_ColorSpaceGamma", gamma);
          //Debug.Log ("Derived gamma = " + gamma);
        }
    }

    void OnDisable() {
        LeapImageRetriever.unregisterImageBasedMaterial(this);
    }
}
