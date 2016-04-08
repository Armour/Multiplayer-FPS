using UnityEngine;
using System.Collections;

public enum LeapDeviceType {
  Invalid,
  Peripheral
}

/// <summary>
/// Leap device info struct.
/// </summary>
/// <remarks>
/// Default values are for Leap peripheral.
/// </remarks>
public struct LeapDeviceInfo {
  public LeapDeviceType type;
  public bool isEmbedded;
  // TODO: Is head mounted
  public float baseline; //(meters) Distance between focal points of cameras
  public float focalPlaneOffset; //(meters) Distance from mount center to focal plane of cameras
  public float horizontalViewAngle; //(degrees) Field of view angle in parallel to baseline axis
  public float verticalViewAngle; //(degrees) Field of view angle perpendicular to baseline axis
  public float trackingRange; //(degrees) Maximum radius for reliable tracking
  public string serialID; //Device alphanumeric unique hardware ID

  public LeapDeviceInfo(LeapDeviceType initialization = LeapDeviceType.Invalid) {
    type = initialization;
    switch (type) {
    case LeapDeviceType.Peripheral:
      isEmbedded = false;
      baseline = 0.04f;
      focalPlaneOffset = 0.07f;
      horizontalViewAngle = 2.303835f * Mathf.Rad2Deg;
      verticalViewAngle = 2.007129f * Mathf.Rad2Deg;
      trackingRange = 470f / 1000f;
      serialID = "";
      break;
    default:
      isEmbedded = false;
      baseline = 0f;
      focalPlaneOffset = 0f;
      horizontalViewAngle = 0f;
      verticalViewAngle = 0f;
      trackingRange = 0f;
      serialID = "";
      break;
    }
  }
}
