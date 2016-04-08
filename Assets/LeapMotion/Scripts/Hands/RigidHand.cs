/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The model for our rigid hand made out of various polyhedra.
public class RigidHand : SkeletalHand {

  public float filtering = 0.5f;

  public override void InitHand() {
    base.InitHand();
  }

  public override void UpdateHand() {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].UpdateFinger();
    }

    if (palm != null) {
      bool useVelocity = false;
      Rigidbody palmBody = palm.GetComponent<Rigidbody>();
      if (palmBody) {
        if (!palmBody.isKinematic) {
          useVelocity = true;

          // Set palm velocity.
          Vector3 target_position = GetPalmCenter();
          palmBody.velocity = (target_position - palm.position) * (1 - filtering) / Time.deltaTime;

          // Set palm angular velocity.
          Quaternion target_rotation = GetPalmRotation();
          Quaternion delta_rotation = target_rotation * Quaternion.Inverse(palm.rotation);
          float angle = 0.0f;
          Vector3 axis = Vector3.zero;
          delta_rotation.ToAngleAxis(out angle, out axis);

          if (angle >= 180) {
            angle = 360 - angle;
            axis = -axis;
          }
          if (angle != 0) {
            float delta_radians = (1 - filtering) * angle * Mathf.Deg2Rad;
            palmBody.angularVelocity = delta_radians * axis / Time.deltaTime;
          }
        }
      }
      if (!useVelocity) {
        palm.position = GetPalmCenter();
        palm.rotation = GetPalmRotation();
      }
    }
    
    if (forearm != null) {
      // Set arm dimensions.
      CapsuleCollider capsule = forearm.GetComponent<CapsuleCollider> ();
      if (capsule != null) {
        // Initialization
        capsule.direction = 2;
        forearm.localScale = new Vector3 (1f, 1f, 1f);
        
        // Update
        capsule.radius = GetArmWidth () / 2f;
        capsule.height = GetArmLength () + GetArmWidth ();
      }
      
      bool useVelocity = false;
      Rigidbody forearmBody = forearm.GetComponent<Rigidbody> ();
      if (forearmBody) {
        if (!forearmBody.isKinematic) {
          useVelocity = true;

          // Set arm velocity.
          Vector3 target_position = GetArmCenter ();
          forearmBody.velocity = (target_position - forearm.position) * (1 - filtering) / Time.deltaTime;

          // Set arm velocity.
          Quaternion target_rotation = GetArmRotation ();
          Quaternion delta_rotation = target_rotation * Quaternion.Inverse (forearm.rotation);
          float angle = 0.0f;
          Vector3 axis = Vector3.zero;
          delta_rotation.ToAngleAxis (out angle, out axis);

          if (angle >= 180) {
            angle = 360 - angle;
            axis = -axis;
          }
          if (angle != 0) {
            float delta_radians = (1 - filtering) * angle * Mathf.Deg2Rad;
            forearmBody.angularVelocity = delta_radians * axis / Time.deltaTime;
          }
        }
      }
      if (!useVelocity) {
        forearm.position = GetArmCenter();
        forearm.rotation = GetArmRotation();
      }
    }
  }
}
