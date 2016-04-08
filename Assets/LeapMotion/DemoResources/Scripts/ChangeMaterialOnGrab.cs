/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class ChangeMaterialOnGrab : MonoBehaviour {

  public Renderer changingObject;
  public Material grabbedMaterial;

  private Material released_material_;

  private bool grabbed_ = false;

  void Start() {
    released_material_ = changingObject.material;
  }

  public void OnGrab() {
    grabbed_ = true;
    changingObject.material = grabbedMaterial;
  }

  public void OnRelease() {
    grabbed_ = false;
    changingObject.material = released_material_;
  }

  void Update() {
    bool grabbed = GetComponent<GrabbableObject>().IsGrabbed();
    if (grabbed && !grabbed_)
      OnGrab();
    else if (!grabbed && grabbed_)
      OnRelease();
  }
}
