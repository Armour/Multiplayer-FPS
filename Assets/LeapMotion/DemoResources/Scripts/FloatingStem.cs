/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class FloatingStem : MonoBehaviour {

  public float waterHeight = 0.0f;
  public float transitionWidth = 0.2f;
  public float waterForce = 0.1f;
  public float waterTorque = 0.002f;
  public float waterDrag = 5.0f;
  public float waterAngularDrag = 10.0f;

  public Vector3 waterCurrentVelocity;
  public float waterCurrentForce = 0.1f;

  private float air_drag_;
  private float air_angular_drag_;

  void Start() {
    air_drag_ = GetComponent<Rigidbody>().drag;
    air_angular_drag_ = GetComponent<Rigidbody>().angularDrag;
  }

  void FixedUpdate() {
    float distanceFromSurface = transform.position.y - waterHeight;
    if (distanceFromSurface >= 0) {
      GetComponent<Rigidbody>().drag = air_drag_;
      GetComponent<Rigidbody>().angularDrag = air_angular_drag_;
      return;
    }

    GetComponent<Rigidbody>().drag = waterDrag;
    GetComponent<Rigidbody>().angularDrag = waterAngularDrag;

    float transition = Mathf.Clamp(-distanceFromSurface / transitionWidth, 0, 1);
    GetComponent<Rigidbody>().AddForce(new Vector3(0, waterForce * transition, 0));

    /*
    float dot = Vector3.Dot(transform.up, Vector3.up);
    Vector3 torque_vector = -dot * Vector3.Cross(transform.up, Vector3.up);
    rigidbody.AddTorque((1 - transition) * waterTorque * torque_vector);
    */

    // Current.
    Vector3 delta_current = waterCurrentVelocity - GetComponent<Rigidbody>().velocity;
    delta_current.y = 0;
    GetComponent<Rigidbody>().AddForce(waterCurrentForce * delta_current);
  }
}
