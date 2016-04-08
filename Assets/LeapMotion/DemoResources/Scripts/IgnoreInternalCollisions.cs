/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class IgnoreInternalCollisions : MonoBehaviour {
	void Start () {
    Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
    for (int i = 0; i < colliders.Length; ++i)
      for (int j = i + 1; j < colliders.Length; ++j)
        Physics.IgnoreCollision(colliders[i], colliders[j]);
	}
}
