/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/**
 * Manages the orientation of the bones in a model rigged for skeletal animation.
 * 
 * The class expects that the graphics model bones corresponding to bones in the Leap Motion 
 * hand model are in the same order in the bones array.
 */
public class RiggedFinger : FingerModel {

  /** Allows the mesh to be stretched to align with finger joint positions
   * Only set to true when mesh is not visible
   */
  public bool deformPosition = false;

  public Vector3 modelFingerPointing = Vector3.forward;
  public Vector3 modelPalmFacing = -Vector3.up;
  
  public Quaternion Reorientation() {
    return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
  }

  /** Updates the bone rotations. */
  public override void UpdateFinger() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        bones[i].rotation = GetBoneRotation(i) * Reorientation();
        if (deformPosition) {
          bones[i].position = GetBoneCenter(i);
        }
      }
    }
  }
}
