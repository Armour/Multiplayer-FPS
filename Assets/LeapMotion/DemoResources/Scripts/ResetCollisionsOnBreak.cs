/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class ResetCollisionsOnBreak : MonoBehaviour {

  public Collider[] collidersToReset;
  
  void OnJointBreak() {
    for (int i = 0; i < collidersToReset.Length; ++i) {
      collidersToReset[i].GetComponent<Collider>().enabled = false;
      collidersToReset[i].GetComponent<Collider>().enabled = true;
    }
  }
}
