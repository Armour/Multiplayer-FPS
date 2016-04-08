using UnityEngine;
using System.Collections;

/// <summary>
/// Provides control of target frame rate.
/// </summary>
/// <remarks>
/// This utility is useful for verifying frame-rate independence of behaviors.
/// </remarks>
public class FrameRateControls : MonoBehaviour {
  public int targetRenderRate = 60; // must be > 0
  public int targetRenderRateStep = 1;
  public int fixedPhysicsRate = 50; // must be > 0
  public int fixedPhysicsRateStep = 1;
  public KeyCode physicsI = KeyCode.RightShift;
  public KeyCode decrease = KeyCode.DownArrow;
  public KeyCode increase = KeyCode.UpArrow;
  public KeyCode resetAll = KeyCode.Delete;

	// Use this for initialization
	void Awake () {
		if (QualitySettings.vSyncCount != 0) {
			Debug.Log("vSync will override target frame rate");
			return;
		}

    Application.targetFrameRate = targetRenderRate;
    Time.fixedDeltaTime = 1f/((float)fixedPhysicsRate);
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKey (physicsI)) {
      if (Input.GetKeyDown (decrease)) {
        if (fixedPhysicsRate > fixedPhysicsRateStep) {
          fixedPhysicsRate -= fixedPhysicsRateStep;
          Time.fixedDeltaTime = 1f/((float)fixedPhysicsRate);
        }
      }
      if (Input.GetKeyDown (increase)) {
        fixedPhysicsRate += fixedPhysicsRateStep;
        Time.fixedDeltaTime = 1f/((float)fixedPhysicsRate);
      }
    } else {
      if (Input.GetKeyDown (decrease)) {
        if (targetRenderRate > targetRenderRateStep) {
          targetRenderRate -= targetRenderRateStep;
          Application.targetFrameRate = targetRenderRate;
        }
      }
      if (Input.GetKeyDown (increase)) {
        targetRenderRate += targetRenderRateStep;
        Application.targetFrameRate = targetRenderRate;
      }
    }
    if (Input.GetKeyDown (resetAll)) {
      Reset();
    }
  }

  public void Reset() {
    targetRenderRate = 60;
    fixedPhysicsRate = 50;
    Application.targetFrameRate = -1;
    Time.fixedDeltaTime = 0.02f;
  }
}
