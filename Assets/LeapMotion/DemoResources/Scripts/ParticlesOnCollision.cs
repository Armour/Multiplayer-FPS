/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class ParticlesOnCollision : MonoBehaviour {

  public float particlesForVelocity = 3.0f;

  void OnCollisionEnter(Collision collision) {
    if (GetComponent<ParticleSystem>() != null)
      GetComponent<ParticleSystem>().Play();
  }
}
