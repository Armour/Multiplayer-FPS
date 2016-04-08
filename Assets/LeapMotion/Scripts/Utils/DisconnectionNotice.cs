/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/**
 * Tracks the connection state of the Leap Motion hardware. If the device is unplugged
 * or otherwise not detected, the script fades in a GUITexture object which should communicate
 * the problem to the user.
 */
public class DisconnectionNotice : MonoBehaviour {

  /** The speed to fade the object alpha from 0 to 1. */
  public float fadeInTime = 1.0f;
  /** The speed to fade the object alpha from 1 to 0. */
  public float fadeOutTime = 1.0f;
  /** The easing curve. */
  public AnimationCurve fade;
  /** A delay before beginning the fade-in effect. */
  public int waitFrames = 10;
  /** An alternative image to use when the hardware is embedded in a keyboard or laptop. */
  public Texture2D embeddedReplacementImage;
  /** The fully on texture tint color. */
  public Color onColor = Color.white;

  private Controller leap_controller_;
  private float fadedIn = 0.0f;
  private int frames_disconnected_ = 0;

  void Start() {
    leap_controller_ = new Controller();
    SetAlpha(0.0f);
  }

  void SetAlpha(float alpha) {
    GetComponent<GUITexture>().color = Color.Lerp(Color.clear, onColor, alpha);
  }

  /** The connection state of the controller. */
  bool IsConnected() {
    return leap_controller_.IsConnected;
  }
  
  /** Whether the controller is embedded in a keyboard or laptop.*/
  bool IsEmbedded() {
    DeviceList devices = leap_controller_.Devices;
    if (devices.Count == 0)
      return false;
    return devices[0].IsEmbedded;
  }
        
  void Update() {
    if (embeddedReplacementImage != null && IsEmbedded()) {
      GetComponent<GUITexture>().texture = embeddedReplacementImage;
    }

    if (IsConnected())
      frames_disconnected_ = 0;
    else
      frames_disconnected_++;

    if (frames_disconnected_ < waitFrames)
      fadedIn -= Time.deltaTime / fadeOutTime;
    else
      fadedIn += Time.deltaTime / fadeInTime;
    fadedIn = Mathf.Clamp(fadedIn, 0.0f, 1.0f);

    SetAlpha(fade.Evaluate(fadedIn));
  }
}
