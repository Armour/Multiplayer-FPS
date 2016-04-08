/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class AerodynamicLeaf : MonoBehaviour {

  public float airDragForce = 0.2f;
  public float airDragTorque = 0.001f;

  // Air to water transition level.
  public float waterHeight = 0.0f;
  public float transitionWidth = 0.2f;

  // Water drag.
  public float waterDrag = 5.0f;
  public float waterAngularDrag = 10.0f;

  // Water forces.
  public float waterBuoancyForce = 0.1f;
  public float waterDragTorque = 0.002f;
  public float waterDragForce = 0.002f;
  public float waterSurfaceTorque = 0.002f;

  // Water curent.
  public Vector3 waterCurrentVelocity;
  public float waterCurrentForce = 0.1f;

  private float air_drag_;
  private float air_angular_drag_;
  private float drag_force_;
  private float drag_torque_;

  void Start() {
    air_drag_ = GetComponent<Rigidbody>().drag;
    air_angular_drag_ = GetComponent<Rigidbody>().angularDrag;
    drag_force_ = airDragForce;
    drag_torque_ = airDragTorque;
  }

  void DragUpdate() {
    Vector3 velocity = GetComponent<Rigidbody>().velocity;
    Vector3 normal = transform.up;

    float dot = Vector3.Dot(velocity, normal);
    GetComponent<Rigidbody>().AddForce(-normal * drag_force_ * dot);

    Vector3 cross = Vector3.Cross(velocity, normal);
    GetComponent<Rigidbody>().AddTorque(-drag_torque_ * cross);
  }

  void AirUpdate() {
    GetComponent<Rigidbody>().drag = air_drag_;
    GetComponent<Rigidbody>().angularDrag = air_angular_drag_;
    drag_force_ = airDragForce;
    drag_torque_ = airDragTorque;
    DragUpdate();
  }

  void WaterUpdate(float level) {
    GetComponent<Rigidbody>().drag = waterDrag;
    GetComponent<Rigidbody>().angularDrag = waterAngularDrag;

    drag_force_ = waterDragForce;
    drag_torque_ = waterDragTorque;
    DragUpdate();

    float transition = Mathf.Clamp(-level / transitionWidth, 0.0f, 1.0f);
    GetComponent<Rigidbody>().AddForce(new Vector3(0, waterBuoancyForce * transition, 0));

    if (Vector3.Dot(transform.up, Vector3.up) >= 0) {
      Vector3 torque_vector = Vector3.Cross(transform.up, Vector3.up);
      GetComponent<Rigidbody>().AddTorque((1 - transition) * waterSurfaceTorque * torque_vector);
    }
    else {
      Vector3 torque_vector = Vector3.Cross(-transform.up, Vector3.up);
      GetComponent<Rigidbody>().AddTorque((1 - transition) * waterSurfaceTorque * torque_vector);
    }

    // Running water current.
    Vector3 delta_current = waterCurrentVelocity - GetComponent<Rigidbody>().velocity;
    delta_current.y = 0;
    GetComponent<Rigidbody>().AddForce(waterCurrentForce * delta_current);
  }
  
  float UnitsAboveWater() {
    return transform.position.y - waterHeight;
  }

  public bool TouchingWater() {
    return UnitsAboveWater() < 0;
  }

  void FixedUpdate() {
    if (TouchingWater())
      WaterUpdate(UnitsAboveWater());
    else
      AirUpdate();
  }
}
