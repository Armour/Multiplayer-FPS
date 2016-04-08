/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// NOTE: This file is a work in progress, changes to come.
/** 
 * Updates a tool model based on tracking data from the Leap Motion service/daemon.
 * In the Leap Motion API, tools represent long, thin objects, like a pencil, which
 * can be tracked by the Leap Motion sensor. Tools are not associated with hands.
 * 
 * A GameObject representing the tool graphics and with this script attached can be added
 * to a HandController's ToolModel slot. The HandController will spawn instances of the game object,
 * updates its position during its lifetime, and finally, destroy the spawned instance when
 * tracking of the tool is lost.
 */
public class ToolModel : MonoBehaviour {

  /** Smoothing factor applied to movement.*/
  public float filtering = 0.5f;

  protected Tool tool_;
  protected HandController controller_;
  protected bool mirror_z_axis_ = false;

  /** The Leap Tool object. */
  public Tool GetLeapTool() {
    return tool_;
  }

  /** Sets the Leap Tool for this ToolModel. */
  public void SetLeapTool(Tool tool) {
    tool_ = tool;
  }

  /** Whether to mirror the tool and motion. */
  public void MirrorZAxis(bool mirror = true) {
    mirror_z_axis_ = mirror;
  }

  /** The Leap Controller object providing tracking data. */
  public HandController GetController() {
    return controller_;
  }

  /** Sets the Leap Controller for this ToolModel. */
  public void SetController(HandController controller) {
    controller_ = controller;
  }
  
  /** The local rotation of this tool based on the tracked tool, the HandController, and the mirror setting.*/
  public Quaternion GetToolRotation() {
    Quaternion local_rotation = Quaternion.FromToRotation(Vector3.forward,
                                                          tool_.Direction.ToUnity(mirror_z_axis_));
    return GetController().transform.rotation * local_rotation;
  }

  /** Calculates the tip velocity of this tool model within the scene. */
  public Vector3 GetToolTipVelocity() {
    Vector3 local_velocity = tool_.TipVelocity.ToUnityScaled(mirror_z_axis_);
    Vector3 total_scale = Vector3.Scale(GetController().handMovementScale,
                                        GetController().transform.localScale);
    Vector3 scaled_velocity = Vector3.Scale(total_scale, local_velocity);
    return GetController().transform.TransformDirection(scaled_velocity);
  }

  /** The position of the tip of this tool in the Unity scene. */ 
  public Vector3 GetToolTipPosition() {
    Vector3 local_point = tool_.TipPosition.ToUnityScaled(mirror_z_axis_);
    Vector3 scaled_point = Vector3.Scale(GetController().handMovementScale, local_point);
    return GetController().transform.TransformPoint(scaled_point);
  }

  /** Initalizes the tool by setting its position and orientation. */
  public void InitTool() {
    transform.position = GetToolTipPosition();
    transform.rotation = GetToolRotation();
  }

  /** Updates the tool by setting its position, velocity, and orientation. */
  public void UpdateTool() {
    Vector3 target_position = GetToolTipPosition();
    if (Time.deltaTime != 0) {
      GetComponent<Rigidbody>().velocity = (target_position - transform.position) *
                           (1 - filtering) / Time.deltaTime;
    }

    // Set angular velocity.
    Quaternion target_rotation = GetToolRotation();
    Quaternion delta_rotation = target_rotation *
                                Quaternion.Inverse(transform.rotation);
    float angle = 0.0f;
    Vector3 axis = Vector3.zero;
    delta_rotation.ToAngleAxis(out angle, out axis);

    if (angle >= 180) {
      angle = 360 - angle;
      axis = -axis;
    }
    if (angle != 0)
      GetComponent<Rigidbody>().angularVelocity = (1 - filtering) * angle * axis;
  }
}
