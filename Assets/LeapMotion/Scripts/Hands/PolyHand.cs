/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/**
* A deforming, very low poly count hand.
*
* All the graphics for this hand are drawn by the fingers. There is no representation
* for the palm or the arm.
*/
public class PolyHand : HandModel {

  /** Initializes the hand and calls the finger initializers. */
  public override void InitHand() {
    SetPalmOrientation();

    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null) {
        fingers[f].fingerType = (Finger.FingerType)f;
        fingers[f].InitFinger();
      }
    }
  }

  /** Updates the hand and calls the finger update functions. */
  public override void UpdateHand() {
    SetPalmOrientation();

    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null) {
        fingers[f].UpdateFinger();
      }
    }
  }

  /** Sets the palm transform. */
  protected void SetPalmOrientation() {
    if (palm != null) {
      palm.position = GetPalmPosition();
      palm.rotation = GetPalmRotation();
    }
  }
}
