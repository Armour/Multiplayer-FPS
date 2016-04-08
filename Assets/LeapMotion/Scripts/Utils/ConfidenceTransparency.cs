/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/** 
 * Updates the hand's opacity based it's confidence rating. 
 * Attach to a HandModel object assigned to the HandController in a scene.
 */
public class ConfidenceTransparency : MonoBehaviour {

  void Update() {
    Hand leap_hand = GetComponent<HandModel>().GetLeapHand();
    float confidence = leap_hand.Confidence;

    if (leap_hand != null) {
      Renderer[] renders = GetComponentsInChildren<Renderer>();
      foreach (Renderer render in renders)
        SetRendererAlpha(render, confidence);
    }
  }

  protected void SetRendererAlpha(Renderer render, float alpha) {
    Color new_color = render.material.color;
    new_color.a = alpha;
    render.material.color = new_color;
  }
}
