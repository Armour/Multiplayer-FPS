using UnityEngine;
using System.Collections;
//using VRWidgets;

namespace LMWidgets
{
  [RequireComponent(typeof(Rigidbody))]
  public class DialModeBase : MonoBehaviour
  {
    [SerializeField]
    private float
      minimumAngle = 0f;
    [SerializeField]
    private float
      maximumAngle = 360f;
    public int steps = 0; // Do not snap to steps when <= 0

    private bool interacting_ = false; // If a GameObject is destroyed OnExit will not be called
    private GameObject target_ = null; // Intersecting object that controls position
    private Vector3 pivot_ = Vector3.zero; // Local position of first intersection

    private float prev_angle_ = 0f;
    private float curr_angle_ = 0f;
    public DialGraphics dialGraphics;
  
    // Standardize Minimum & Maximum
    void Awake ()
    {
      //GetComponent<Collider>().isTrigger = true;
      GetComponent<Rigidbody>().isKinematic = true;
      if (minimumAngle == maximumAngle) {
        // Use default ranges
        minimumAngle = 0f;
        maximumAngle = 360f;
        return;
      }
      if (minimumAngle > maximumAngle) {
        // Ensure correct orientation
        float swap = minimumAngle;
        minimumAngle = maximumAngle;
        maximumAngle = swap;
      }
      float over = maximumAngle - minimumAngle - 360f;
      if (over > 0f) {
        // restrict range
        maximumAngle -= over / 2f;
        minimumAngle += over / 2f;
      }
    }

    void OnEnable ()
    {
      if (steps > 0) 
        SnapToStep ();
    }

    void OnDisable ()
    {
      // Avoid lingering references to exited objects
      //if (interacting_) Debug.Log ("NEVER STOPPED INTERACTING");
      //if (target_ != null) Debug.Log ("TARGET WAS NOT RELEASED");
      EndInteraction ();
    }

    // Maps angle to the range (-180, 180]
    protected float MinAngleToZero (float angle)
    {
      float minAngle = angle % 360f;
      if (minAngle > 180f)
        minAngle = minAngle - 360f;
      if (minAngle <= -180f)
        minAngle = 360f + minAngle;
      return minAngle;
    }
  
    // Restrictions to min & max evenly divide the out-of-range angles
    protected float RestrictAngle (float setAngle)
    {
      float acceptDivider = (maximumAngle + minimumAngle) / 2f; // midpoint of acceptance region
      float resAngle = MinAngleToZero (setAngle - acceptDivider);
      if (resAngle > maximumAngle - acceptDivider)
        resAngle = maximumAngle - acceptDivider;
      if (resAngle < minimumAngle - acceptDivider)
        resAngle = minimumAngle - acceptDivider;
      resAngle += acceptDivider;
      return resAngle;
    }

    protected int RestrictStep (float setAngle)
    {
      float setFraction = (RestrictAngle (setAngle) - minimumAngle) / (maximumAngle - minimumAngle);
      int resStep = (int)((steps * setFraction) + 0.5f);
      if (resStep == steps)
        // When setAngle == maximumAngle int rounding will not yield a reduction
        resStep = steps - 1;
      return resStep;
    }

    // CurrentAngle ranges from minimumAngle to maximumAngle
    public float CurrentAngle {
      get {
        return RestrictAngle (transform.localRotation.eulerAngles.y);
      }
      set {
//		Debug.Log ("DialModeBase.CurrentAngle is being set to: " + value);		
		Vector3 eulerAngles = transform.localRotation.eulerAngles;
        eulerAngles.x = 0f;
        eulerAngles.y = RestrictAngle (value);
        eulerAngles.z = 0f;
        transform.localRotation = Quaternion.Euler (eulerAngles);
      }
    }

    // CurrentStep ranges from 0 to steps-1
    public int CurrentStep {
      get {
        return RestrictStep (transform.localRotation.eulerAngles.y);
      }
      set {
//      	Debug.Log ("DialModeBase.CurrentStep is being set to: " + value);
        if (steps <= 0)
          return;
        CurrentAngle = (value * (maximumAngle - minimumAngle) / steps) + minimumAngle;
      }
    }

    private bool IsHand (Collider other)
    {
      return other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel> ();
    }

    void OnTriggerEnter (Collider other)
    {
      if (target_ == null && IsHand (other)) {
        target_ = other.gameObject;
        pivot_ = transform.InverseTransformPoint (target_.transform.position) - transform.localPosition;
        if (GetComponent<Rigidbody>().isKinematic == false)
          transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        interacting_ = true;
        if (dialGraphics)
          dialGraphics.HilightDial ();
      }
    }

    void OnTriggerExit (Collider other)
    {
      if (other.gameObject == target_) {
        EndInteraction ();
      }
    }

    void EndInteraction ()
    {
      target_ = null;
      if (steps > 0) {
        SnapToStep ();
      } else {
        float FPS_INVERSE = 1f / Time.deltaTime;
        float angular_velocity = (curr_angle_ - prev_angle_) * FPS_INVERSE;
        transform.GetComponent<Rigidbody>().AddRelativeTorque (new Vector3 (0f, 0f, angular_velocity));
      }
      interacting_ = false;
      // NOTE: External update should following internal state update,
      // so that exceptions in external update do not yield inconsistent state
      if (dialGraphics)
        dialGraphics.UpdateDial ();
    }
    
    protected virtual void ApplyRotations ()
    {
      Vector3 curr_direction = transform.InverseTransformPoint (target_.transform.position) - transform.localPosition;
      transform.localRotation = Quaternion.FromToRotation (pivot_, curr_direction) * transform.localRotation;
    }

    protected virtual void ApplyConstraints ()
    {
      Vector3 rotation = transform.localRotation.eulerAngles;
      rotation.x = 0f;
      // Allow dial manipulation to exceed range - it will snap back when released
      prev_angle_ = curr_angle_;
      curr_angle_ = rotation.y;
      rotation.z = 0f;
      transform.localRotation = Quaternion.Euler (rotation);
    }

    protected virtual void SnapToStep ()
    {
      CurrentStep = CurrentStep;
      ApplyConstraints ();
    }

    void FixedUpdate ()
    {
      if (target_ == null && interacting_) {
        // Hand destroyed while interacting
        //Debug.Log ("HAND DESTROYED WHILE INTERACTING");
        EndInteraction ();
      }
      if (target_ != null) {  
        ApplyRotations ();
      }
      ApplyConstraints ();
    }
  }
}

