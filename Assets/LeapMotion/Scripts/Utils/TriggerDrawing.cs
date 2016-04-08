/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// Leap Motion hand script that detects pinches and grabs the
// closest rigidbody with a spring force if it's within a given range.
public class TriggerDrawing : MonoBehaviour {

  public const float TRIGGER_DISTANCE_RATIO = 0.7f;

  public float lineWidth = 1.0f;
  public float minDistanceForNewPoint = 0.1f;

  protected LineRenderer current_line_;
  protected int current_line_length_ = 0;
  protected List<LineRenderer> lines_;

  protected bool drawing_;
  protected Collider grabbed_;
  protected Vector3 last_draw_position_ = Vector3.zero;

  void Start() {
    drawing_ = false;
    grabbed_ = null;
    lines_ = new List<LineRenderer>();
  }

  void StartNewLine(Vector3 draw_position) {
    GameObject line_object = new GameObject();
    LineRenderer line_model = GetComponent<LineRenderer>();
    current_line_ = line_object.AddComponent<LineRenderer>();
    current_line_.materials = line_model.materials;
    current_line_.castShadows = line_model.castShadows;
    current_line_.receiveShadows = line_model.receiveShadows;
    current_line_.SetWidth(lineWidth, lineWidth);

    current_line_length_ = 0;
    lines_.Add(current_line_);
    last_draw_position_ = Vector3.zero;
    ContinueLine(draw_position);
  }

  void ContinueLine(Vector3 draw_position) {
    if (Vector3.Distance(draw_position, last_draw_position_) < minDistanceForNewPoint)
      return;
    current_line_length_++;
    current_line_.SetVertexCount(current_line_length_);
    current_line_.SetPosition(current_line_length_ - 1, draw_position);
    last_draw_position_ = draw_position;
  }

  void OnDestroy() {
    foreach (LineRenderer line in lines_)
      Destroy(line.gameObject);
  }

  void Update() {
    bool trigger = false;
    HandModel hand_model = GetComponent<HandModel>();
    Hand leap_hand = hand_model.GetLeapHand();

    if (leap_hand == null)
      return;

    // Scale trigger distance by thumb proximal bone length.
    Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
    float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
    float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger; ++i) {
      Finger finger = leap_hand.Fingers[i];

      for (int j = 0; j < FingerModel.NUM_BONES && !trigger; ++j) {
        Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
        if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
          trigger = true;
      }
    }

    Vector3 draw_position = hand_model.fingers[1].GetTipPosition();

    // Only change state if it's different.
    if (trigger && !drawing_)
      StartNewLine(draw_position);
    
    if (drawing_)
      ContinueLine(draw_position);

    drawing_ = trigger;
  }
}
