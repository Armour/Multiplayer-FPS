/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class HandCycler : MonoBehaviour {

  public HandModel[] leftHands;
  public HandModel[] rightHands;

  private int hand_index_ = 0;

  void Start() {
    SetNewHands();
  }

  protected void SetNewHands() {
    HandController controller = GetComponent<HandController>();
    controller.leftGraphicsModel = leftHands[hand_index_];
    controller.rightGraphicsModel = rightHands[hand_index_];
    controller.DestroyAllHands();
  }

  void OnGUI() {
    Event current_event = Event.current;
    if (current_event.type == EventType.KeyDown) {
      if (current_event.keyCode == KeyCode.LeftArrow) {
        hand_index_ = (hand_index_ + leftHands.Length - 1) % leftHands.Length;
        SetNewHands();
      }
      else if (current_event.keyCode == KeyCode.RightArrow) {
        hand_index_ = (hand_index_ + 1) % leftHands.Length;
        SetNewHands();
      }
    }
  }
}
