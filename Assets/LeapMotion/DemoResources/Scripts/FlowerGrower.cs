/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class FlowerGrower : MonoBehaviour {

  public float growthRate = 1.0f;
  public float deathRate = 1.0f;
  public float growthProgress = 0.0f;

  public StemMesh stem;
  public float stemStartGrowth = 0.0f;
  public float stemEndGrowth = 0.6f;

  public Transform flowerHead;
  public float flowerHeadStartGrowth = 0.5f;
  public float flowerHeadEndGrowth = 0.8f;

  public Light lightSource;
  public float lightSourceStartGrowth = 0.0f;
  public float lightSourceEndGrowth = 0.8f;

  public PetalMesh[] leaves;
  public float leavesStartGrowth = 0.4f;
  public float leavesEndGrowth = 0.8f;

  public PetalMesh[] pedals;
  public float pedalsStartGrowth = 0.8f;
  public float pedalsEndGrowth = 1.0f;

  public FlowerBloom flowerToBloom;

  private Vector3 flower_head_scale_ = Vector3.zero;
  private float light_source_intensity_ = 0.0f;
  private bool growing_ = true;
  private bool dieing_ = false;

  void Start() {
    flower_head_scale_ = flowerHead.localScale;
    light_source_intensity_ = lightSource.intensity;
    flowerHead.localScale = Vector3.zero;

    foreach (PetalMesh pedal in pedals)
      pedal.growthProgress = 0;

    foreach (PetalMesh leaf in leaves)
      leaf.growthProgress = 0;
  }

  float ComputeGrowthAmount(float start, float end) {
    return Mathf.Clamp((growthProgress - start) / (end - start), 0.0f, 1.0f);
  }

  void SetSizes() {
    stem.growthProgress = ComputeGrowthAmount(stemStartGrowth, stemEndGrowth);

    float flower_head_growth = ComputeGrowthAmount(flowerHeadStartGrowth, flowerHeadEndGrowth);
    flowerHead.localScale = flower_head_growth * flower_head_scale_;

    float light_source_growth = ComputeGrowthAmount(lightSourceStartGrowth, lightSourceEndGrowth);
    lightSource.intensity = light_source_growth * light_source_intensity_;

    float leaf_growth = ComputeGrowthAmount(leavesStartGrowth, leavesEndGrowth);
    foreach (PetalMesh leaf in leaves)
      leaf.growthProgress = leaf_growth;

    float pedal_growth = ComputeGrowthAmount(pedalsStartGrowth, pedalsEndGrowth);
    foreach (PetalMesh pedal in pedals)
      pedal.growthProgress = pedal_growth;
  }

  public void RemoveStump() {
    stem.RemoveStump();
  }

  public bool IsStumpClear() {
    return stem.IsStumpClear();
  }

  public void Die() {
    dieing_ = true;
  }

  public bool IsDead() {
    return growthProgress == 0.0f;
  }

  public bool IsBroken() {
    return stem.IsBroken();
  }
  
  public bool IsGrabbed() {
    GrabbableObject[] grabbables = GetComponentsInChildren<GrabbableObject>();
    foreach (GrabbableObject grabbable in grabbables) {
      if (grabbable.IsGrabbed())
        return true;
    }
    return false;
  }

  void Update() {
    if (dieing_)
      growthProgress = Mathf.Clamp(growthProgress - Time.deltaTime * deathRate, 0.0f, 1.0f);
    else if (growing_)
      growthProgress = Mathf.Clamp(growthProgress + Time.deltaTime * growthRate, 0.0f, 1.0f);

    SetSizes();

    if (growthProgress == 1.0f && flowerToBloom != null)
      flowerToBloom.open = true;
  }
}
