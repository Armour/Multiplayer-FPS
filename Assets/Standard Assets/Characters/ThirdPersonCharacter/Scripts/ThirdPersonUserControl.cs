using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace UnitySampleAssets.Characters.ThirdPerson
{

    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {

        public bool walkByDefault = false; // toggle for walking state

        public bool lookInCameraDirection = true;// should the character be looking in the same direction that the camera is facing

        private Vector3 lookPos; // The position that the character should be looking towards
        private ThirdPersonCharacter character; // A reference to the ThirdPersonCharacter on the object
        private Transform cam; // A reference to the main camera in the scenes transform
        private Vector3 camForward; // The current forward direction of the camera

        private Vector3 move;
        private bool jump;// the world-relative desired move direction, calculated from the camForward and user input.

        // Use this for initialization
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<ThirdPersonCharacter>();
        }

        void Update()
        {
            if(!jump)
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            bool crouch = false;

            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (cam != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                move = v*camForward + h*cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                move = v*Vector3.forward + h*Vector3.right;
            }

            if (move.magnitude > 1) move.Normalize();

#if !MOBILE_INPUT
            // On non-mobile builds, walk/run speed is modified by a key press.
            bool walkToggle = Input.GetKey(KeyCode.LeftShift);
            // We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
            float walkMultiplier = (walkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);
            move *= walkMultiplier;
#endif

            // calculate the head look target position
            lookPos = lookInCameraDirection && cam != null
                          ? transform.position + cam.forward*100
                          : transform.position + transform.forward*100;

            // pass all parameters to the character control script
            character.Move(move, crouch, jump, lookPos);
            jump = false;
        }
    }
}