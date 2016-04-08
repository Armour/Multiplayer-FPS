/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/**
* The finger model for our debugging. Draws debug lines for each bone.
*/
public class DebugFinger : FingerModel {

  /** The colors used for each bone. */
  protected Color[] colors = {Color.yellow, Color.green, Color.cyan, Color.blue};

  /** Updates the finger and calls the line drawing function. */
  public override void UpdateFinger() {
    DrawDebugLines();
  }

  /**
  * Draws a line from joint to joint.
  */
  protected void DrawDebugLines() {
    for (int i = 0; i < NUM_BONES; ++i)
      Debug.DrawLine(GetJointPosition(i), GetJointPosition(i + 1), colors[i]);
  }
}
