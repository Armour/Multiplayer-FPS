using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Fade out text label in dial as it rotates away.
/// </summary>
/// <remarks>
/// Uses the dot product between the parent of the label's forward direction
/// and the forward direction of the label, passed through a curve filter 
/// to determine the opacity to set the text label to.
/// </remarks>
public class FadeTextByRotation : MonoBehaviour {
  /// <summary>
  /// Curve to translate dot product to opacity
  /// </summary>
  public AnimationCurve FadeCurve;

  /// <summary>
  /// Reference for the "forward direction. AutoDiscovered.
  /// </summary>
  /// <remarks>
  /// AutoDiscovery:
  /// Uses the label's parent's parent (label -> dial center -> panel center)
  /// 
  /// Autodiscovery can be overrriden by assigning the reference transform in the editor.
  /// 
  /// "Forward" is assumed to be -z.
  /// </remarks>
  public Transform ReferenceTransform_AutoDiscovered;

  /// <summary>
  /// The starting opacity of the label.
  /// </summary>
  private float m_originalLabelOpacity;

  /// <summary>
  /// Cache a reference to all underlying text labels.
  /// </summary>
  /// <remarks>
  /// We want to cache this both to avoid the extra call
  /// to GetComponentsInChildren and to avoid the extra array
  /// alloc on the heap.
  /// </remarks>
  private Text[] m_textLabels;


  /// <summary>
  /// Finds and assigns a reference to the reference transform.
  /// </summary>
  /// <returns>
  /// Returns true if the registration was successful or already complete.
  /// Returns false if the registration failed and the reference transform is still null.
  /// </returns>
  private bool registerReferenceTransform() {
    if (ReferenceTransform_AutoDiscovered != null) {
      return true;
    }
    if (transform.parent == null) { return false; }
    ReferenceTransform_AutoDiscovered = transform.parent.parent;
    return ReferenceTransform_AutoDiscovered != null;
  }

  void Awake() {
    m_textLabels = GetComponentsInChildren<Text>(true);

    if(m_textLabels.Length == 0) { 
      Debug.LogWarning("No text labels detected. Nothing to fade.");
      return; 
    }

    // Using a relatively naive selection process here. 
    // As of writing this there is only one label, but writing this 
    // to support [n] labels because it is trivial.
    m_originalLabelOpacity = m_textLabels[0].color.a;
  }

  void OnEnable() {
    if (ReferenceTransform_AutoDiscovered == null) {
      registerReferenceTransform();
    }
  }
	
	// Update is called once per frame
	void Update () {
    // Make sure there is a reference transform to reference.
    if (ReferenceTransform_AutoDiscovered == null) {
      bool registered = registerReferenceTransform();
      if (!registered) {
        Debug.LogError("No reference transform. Exiting.");
        return;
      }
    }

    // Make sure there are text labels to operate on.
    if (m_textLabels.Length == 0) {
      return;
    }

    float referenceDotDirection = Vector3.Dot(ReferenceTransform_AutoDiscovered.forward, transform.forward);
    referenceDotDirection = Mathf.Clamp01(referenceDotDirection);
    
    // We say opacity mod because the actual opacity will be 
    // the original opacity * the opacity mod.
    // The original opacity is assumed to be the max opacity.
    float opacityMod = FadeCurve.Evaluate(referenceDotDirection);
    float goalOpacity = m_originalLabelOpacity * opacityMod;

    // ForEach over an array is memory-optimized in Unity so we can use it. 
    // Usually want to avoid this because of spurious allocs due to the enumerator.
    foreach(Text textComponent in m_textLabels) {
      Color textColor = textComponent.color;
      textColor.a = goalOpacity;
      textComponent.color = textColor;
    }
	}
}
