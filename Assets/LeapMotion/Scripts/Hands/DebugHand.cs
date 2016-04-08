/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/**
 * A HandModel that draws lines for the bones in the hand and its fingers.
 * 
 * The debugs lines are only drawn in the Editor Scene view (when a hand is tracked) and
 * not in the Game view. Use debug hands when you aren't using visible hands in a scene
 * so that you can see where the hands are in the scene view.
 * */
public class DebugHand : HandModel {

  /**
  * Initializes the hand and calls the line drawing function.
  */
  public override void InitHand() {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].InitFinger();
    }

    DrawDebugLines();
  }

  /**
  * Updates the hand and calls the line drawing function.
  */
  public override void UpdateHand() {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].UpdateFinger();
    }

    DrawDebugLines();
  }

  /**
  * Draws lines from elbow to wrist, wrist to palm, and normal to the palm.
  */
  protected void DrawDebugLines() {
    HandModel hand = GetComponent<HandModel>();
    Debug.DrawLine(hand.GetElbowPosition(), hand.GetWristPosition(), Color.red);
    Debug.DrawLine(hand.GetWristPosition(), hand.GetPalmPosition(), Color.white);
    Debug.DrawLine(hand.GetPalmPosition(),
                   hand.GetPalmPosition() + hand.GetPalmNormal(), Color.black);
    Debug.Log(Vector3.Dot(hand.GetPalmDirection(), hand.GetPalmNormal()));
  }
}
