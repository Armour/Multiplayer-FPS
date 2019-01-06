using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnitySampleAssets.Cameras
{
    [ExecuteInEditMode]
    public class AutoCam : PivotBasedCameraRig
    {
        [SerializeField] private float moveSpeed = 3; // How fast the rig will move to keep up with target's position
        [SerializeField] private float turnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float rollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] private bool followVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField] private bool followTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField] private float spinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField] private float targetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] private float smoothTurnTime = 0.2f; // the smoothing for the camera's rotation

        private float lastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float currentTurnAmount; // How much to turn the camera
        private float turnSpeedVelocityChange; // The change in the turn speed velocity

        private Vector3 rollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

        protected override void FollowTarget(float deltaTime)
        {
            // if no target, or no time passed then we quit early, as there is nothing to do
            if (!(deltaTime > 0) || target == null)
            {
                return;
            }

            // initialise some vars, we'll be modifying these in a moment
            var targetForward = target.forward;
            var targetUp = target.up;

            if (followVelocity && Application.isPlaying)
            {
                // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
                // but only if the object is traveling faster than a given threshold.

                if (target.GetComponent<Rigidbody>().velocity.magnitude > targetVelocityLowerLimit)
                {
                    // velocity is high enough, so we'll use the target's velocty
                    targetForward = target.GetComponent<Rigidbody>().velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, 1, ref turnSpeedVelocityChange, smoothTurnTime);
            }
            else
            {
                // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

                // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
                // eg when a car has been knocked into a spin. The camera will resume following the rotation
                // of the target when the target's angular velocity slows below the threshold.
                var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z)*Mathf.Rad2Deg;
                if (spinTurnLimit > 0)
                {
                    var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(lastFlatAngle, currentFlatAngle))/deltaTime;
                    var desiredTurnAmount = Mathf.InverseLerp(spinTurnLimit, spinTurnLimit*0.75f, targetSpinSpeed);
                    var turnReactSpeed = (currentTurnAmount > desiredTurnAmount ? .1f : 1f);
                    if (Application.isPlaying)
                    {
                        currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, desiredTurnAmount,
                                                             ref turnSpeedVelocityChange, turnReactSpeed);
                    }
                    else
                    {
                        // for editor mode, smoothdamp won't work because it uses deltaTime internally
                        currentTurnAmount = desiredTurnAmount;
                    }
                }
                else
                {
                    currentTurnAmount = 1;
                }
                lastFlatAngle = currentFlatAngle;
            }

            // camera position moves towards target position:
            transform.position = Vector3.Lerp(transform.position, target.position, deltaTime*moveSpeed);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
            if (!followTilt)
            {
                targetForward.y = 0;
                if (targetForward.sqrMagnitude < float.Epsilon)
                {
                    targetForward = transform.forward;
                }
            }
            var rollRotation = Quaternion.LookRotation(targetForward, rollUp);

            // and aligning with the target object's up direction (i.e. its 'roll')
            rollUp = rollSpeed > 0 ? Vector3.Slerp(rollUp, targetUp, rollSpeed*deltaTime) : Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, turnSpeed*currentTurnAmount*deltaTime);
        }
    }
}
