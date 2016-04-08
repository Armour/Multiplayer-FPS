using UnityEngine;
using System.Collections;
using Leap;

public class HandDetector : MonoBehaviour {

  public HandController leap_controller_;

  HandModel GetHand(Collider other)
  {
    HandModel hand_model = null;
    // Navigate a maximum of 3 levels to find the HandModel component.
    int level = 1;
    Transform parent = other.transform.parent;
    while (parent != null && level < 3) {
      hand_model = parent.GetComponent<HandModel>();
      if (hand_model != null) {
        break;
      }
      parent = parent.parent;
    }

    return hand_model;
  }

  // Finds the first instance (by depth-firstrecursion)
  // of a child with the specified name
  Transform FindPart(Transform parent, string name) {
    if (parent == null) {
      return parent;
    }
    if (parent.name == name) {
      return parent;
    }
    for (int c = 0; c < parent.childCount; c++) {
      Transform part = FindPart(parent.GetChild(c), name);
      if (part != null) {
        return part;
      }
    }
    return null;
  }

  void OnTriggerEnter(Collider other)
  {
    HandModel hand_model = GetHand(other);
    if (hand_model != null)
    {
      int handID = hand_model.GetLeapHand().Id;
      HandModel[] hand_models = leap_controller_.GetAllGraphicsHands();
      for (int i = 0; i < hand_models.Length; ++i)
      {
        if (hand_models[i].GetLeapHand().Id == handID)
        {
          Transform part = null;
          if (other.transform.parent.GetComponent<HandModel>() != null) {
            // Palm or Forearm components
            part = FindPart(hand_models[i].transform, other.name);
          } else if (other.transform.parent.GetComponent<FingerModel>() != null) {
            // Bone in a finger
            part = FindPart(FindPart(hand_models[i].transform, other.transform.parent.name), other.name);
          }
          //Debug.Log ("Detected: " + other.transform.parent.name + "/" + other.gameObject.name);
          if (part != null) {
            Renderer[] renderers = part.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in renderers) {
              //Debug.Log ("Marked: " + renderer.gameObject.transform.parent.name + "/" + renderer.gameObject.name);
              renderer.material.color = Color.red;
            }
          }
        }
      }
    }
  }
}
