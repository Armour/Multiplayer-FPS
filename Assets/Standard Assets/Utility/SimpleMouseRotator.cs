using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;


namespace UnitySampleAssets.Utility
{

    public class SimpleMouseRotator : MonoBehaviour
    {

        // A mouselook behaviour with constraints which operate relative to
        // this gameobject's initial rotation.

        // Only rotates around local X and Y.

        // Works in local coordinates, so if this object is parented
        // to another moving gameobject, its local constraints will
        // operate correctly
        // (Think: looking out the side window of a car, or a gun turret
        // on a moving spaceship with a limited angular range)

        // to have no constraints on an axis, set the rotationRange to 360 or greater.

        public Vector2 rotationRange = new Vector3(70, 70);
        public float rotationSpeed = 10;
        public float dampingTime = 0.2f;
        public bool autoZeroVerticalOnMobile = true;
        public bool autoZeroHorizontalOnMobile = false;
        public bool relative = true;
        private Vector3 targetAngles;
        private Vector3 followAngles;
        private Vector3 followVelocity;
        private Quaternion originalRotation;


        // Use this for initialization
        private void Start()
        {
            originalRotation = transform.localRotation;
        }

        // Update is called once per frame
        private void Update()
        {

            // we make initial calculations from the original local rotation
            transform.localRotation = originalRotation;

            // read input from mouse or mobile controls
            float inputH = 0;
            float inputV = 0;
            if (relative)
            {

                inputH = CrossPlatformInputManager.GetAxis("Mouse X");
                inputV = CrossPlatformInputManager.GetAxis("Mouse Y");

                // wrap values to avoid springing quickly the wrong way from positive to negative
                if (targetAngles.y > 180)
                {
                    targetAngles.y -= 360;
                    followAngles.y -= 360;
                }
                if (targetAngles.x > 180)
                {
                    targetAngles.x -= 360;
                    followAngles.x -= 360;
                }
                if (targetAngles.y < -180)
                {
                    targetAngles.y += 360;
                    followAngles.y += 360;
                }
                if (targetAngles.x < -180)
                {
                    targetAngles.x += 360;
                    followAngles.x += 360;
                }

#if MOBILE_INPUT
    // on mobile, sometimes we want input mapped directly to tilt value,
    // so it springs back automatically when the look input is released.
			if (autoZeroHorizontalOnMobile) {
				targetAngles.y = Mathf.Lerp (-rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH * .5f + .5f);
			} else {
				targetAngles.y += inputH * rotationSpeed;
			}
			if (autoZeroVerticalOnMobile) {
				targetAngles.x = Mathf.Lerp (-rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV * .5f + .5f);
			} else {
				targetAngles.x += inputV * rotationSpeed;
			}
#else
                // with mouse input, we have direct control with no springback required.
                targetAngles.y += inputH*rotationSpeed;
                targetAngles.x += inputV*rotationSpeed;
#endif

                // clamp values to allowed range
                targetAngles.y = Mathf.Clamp(targetAngles.y, -rotationRange.y*0.5f, rotationRange.y*0.5f);
                targetAngles.x = Mathf.Clamp(targetAngles.x, -rotationRange.x*0.5f, rotationRange.x*0.5f);

            }
            else
            {

                inputH = Input.mousePosition.x;
                inputV = Input.mousePosition.y;

                // set values to allowed range
                targetAngles.y = Mathf.Lerp(-rotationRange.y*0.5f, rotationRange.y*0.5f, inputH/Screen.width);
                targetAngles.x = Mathf.Lerp(-rotationRange.x*0.5f, rotationRange.x*0.5f, inputV/Screen.height);
            }

            // smoothly interpolate current values to target angles
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, dampingTime);

            // update the actual gameobject's rotation
            transform.localRotation = originalRotation*Quaternion.Euler(-followAngles.x, followAngles.y, 0);
        }
    }
}
