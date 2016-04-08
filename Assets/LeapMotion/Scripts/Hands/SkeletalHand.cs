/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/** 
 * A hand object consisting of discrete, component parts.
 * 
 * The hand can have game objects for the palm, wrist and forearm, as well as fingers.
 */
public class SkeletalHand : HandModel {

  protected const float PALM_CENTER_OFFSET = 0.0150f;

  void Start() {
    // Ignore collisions with self.
    Leap.Utils.IgnoreCollisions(gameObject, gameObject);

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null) {
        fingers[i].fingerType = (Finger.FingerType)i;
      }
    }
  }

  /** Updates the hand and its component parts by setting their positions and rotations. */
  public override void UpdateHand() {
    SetPositions();
  }

  protected Vector3 GetPalmCenter() {
    Vector3 offset = PALM_CENTER_OFFSET * Vector3.Scale(GetPalmDirection(), transform.localScale);
    return GetPalmPosition() - offset;
  }

  protected void SetPositions() {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].UpdateFinger();
    }

    if (palm != null) {
      palm.position = GetPalmCenter();
      palm.rotation = GetPalmRotation();
    }

    if (wristJoint != null) {
      wristJoint.position = GetWristPosition();
      wristJoint.rotation = GetPalmRotation();
    }

    if (forearm != null) {
      forearm.position = GetArmCenter();
      forearm.rotation = GetArmRotation();
    }
  }
}


