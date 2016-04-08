using UnityEngine;

namespace LMWidgets
{
  /// <summary>
  /// Base class for spring. Restrains the widget in its local z-axis.
  /// It will apply spring physics in ApplyPhysics and translate the button with hand in ApplyInteractions
  /// </summary>
  public abstract class LeapPhysicsSpring : LeapPhysicsBase
  {
    /// <summary>
    /// Spring constant is separated to xyz-axis for more flexible configuration
    /// </summary>
    public Vector3 springCoefficient = Vector3.one * 10.0f;
    public bool applyCriticalDamping = true;

    private Vector3 m_dampingCoefficient = Vector3.zero;
    private Vector3 m_interactionConstraints = Vector3.one;

    /// <summary>
    /// Applies Interaction constraints. Takes in a Vector3. If an axis has value > 0.5 then it's allowed to move. Otherwise it won't be
    /// </summary>
    /// <param name="interactionConstraints"></param>
    protected void ApplyInteractionConstraints(Vector3 interactionConstraints)
    {
      interactionConstraints.x = (interactionConstraints.x > 0.5f) ? 1.0f : 0.0f;
      interactionConstraints.y = (interactionConstraints.y > 0.5f) ? 1.0f : 0.0f;
      interactionConstraints.z = (interactionConstraints.z > 0.5f) ? 1.0f : 0.0f;
      m_interactionConstraints = interactionConstraints;
      ResetPivots();
    }

    /// <summary>
    /// Apply spring physics
    /// </summary>
    protected override void ApplyPhysics()
    {
      Vector3 springForce = Vector3.Scale(-springCoefficient, transform.localPosition);
      Vector3 dampingForce = Vector3.zero;

      if (applyCriticalDamping)
      {
        Vector3 instantVelocity = springForce * Time.deltaTime;
        dampingForce = Vector3.Scale(-m_dampingCoefficient, instantVelocity);
      }
      transform.localPosition += (springForce + dampingForce) * Time.deltaTime;
    }

    /// <summary>
    /// Translate the widget with the hand during interaction
    /// </summary>
    protected override void ApplyInteractions()
    {
      Vector3 displacement = Vector3.Scale(transform.parent.InverseTransformPoint(m_target.transform.position) - m_targetPivot, m_interactionConstraints);
      transform.localPosition = displacement + m_pivot;
    }

    protected override void Awake()
    {
      base.Awake();
      Vector3 k = springCoefficient;
      m_dampingCoefficient = 2 * new Vector3(Mathf.Sqrt(k.x), Mathf.Sqrt(k.y), Mathf.Sqrt(k.z));
    }
  }
}
