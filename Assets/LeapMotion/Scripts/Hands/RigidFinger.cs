/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The finger model for our rigid hand made out of various cubes.
public class RigidFinger : SkeletalFinger {

  public float filtering = 0.5f;

  void Start() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        bones[i].GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
      }
    }
  }

  public override void UpdateFinger() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        // Set bone dimensions.
        CapsuleCollider capsule = bones[i].GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
          // Initialization
          capsule.direction = 2;
          bones[i].localScale = new Vector3(1f, 1f, 1f);

          // Update
          capsule.radius = GetBoneWidth(i) / 2f;
          capsule.height = GetBoneLength(i) + GetBoneWidth(i);
        }

        bool useVelocity = false;
        Rigidbody boneBody = bones[i].GetComponent<Rigidbody>();
        if (boneBody) {
          if (!boneBody.isKinematic) {
            useVelocity = true;

            // Set velocity.
            Vector3 target_bone_position = GetBoneCenter(i);
            
            bones[i].GetComponent<Rigidbody>().velocity = (target_bone_position - bones[i].position) * ((1 - filtering) / Time.deltaTime);

            // Set angular velocity.
            Quaternion target_rotation = GetBoneRotation(i);
            Quaternion delta_rotation = target_rotation * Quaternion.Inverse(bones[i].rotation);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            delta_rotation.ToAngleAxis(out angle, out axis);

            if (angle >= 180) {
              angle = 360 - angle;
              axis  = -axis;
            }

            if (angle != 0) {
              float delta_radians = (1 - filtering) * angle * Mathf.Deg2Rad;
              bones[i].GetComponent<Rigidbody>().angularVelocity = delta_radians * axis / Time.deltaTime;
            }
          }
        }
        if (!useVelocity) {
          bones[i].position = GetBoneCenter(i);
          bones[i].rotation = GetBoneRotation(i);
        }
      }
    }
  }
}
