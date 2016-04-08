using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Execution ordering is determined by enable ordering.
/// Provided that scripts can begin disabled, this will
/// activate scripts in the desired order.
/// </summary>
public class EnableUpdateOrdering : MonoBehaviour {
  public List<MonoBehaviour> ordering;

	// Use this for initialization
	void Start () {
    foreach (MonoBehaviour script in ordering) {
      if (script == null)
        continue;
      script.enabled = true;
    }
	}
}
