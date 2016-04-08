using Leap;
using UnityEngine;
using System.Collections;


public class HandFader : MonoBehaviour {
    public float confidenceSmoothing = 10.0f;
    public AnimationCurve confidenceCurve;

    private HandModel _handModel;
    private float _smoothedConfidence = 0.0f;
    private Renderer _renderer;

    void Awake() {
        _handModel = GetComponent<HandModel>();
        _renderer = GetComponentInChildren<Renderer>();
        _renderer.material.SetFloat("_Fade", 0);
    }

	void Update () {
        _smoothedConfidence += (_handModel.GetLeapHand().Confidence - _smoothedConfidence) / confidenceSmoothing;
        float fade = confidenceCurve.Evaluate(_smoothedConfidence);
        _renderer.enabled = fade != 0.0f;
        _renderer.material.SetFloat("_Fade", confidenceCurve.Evaluate(_smoothedConfidence));
	}
}
