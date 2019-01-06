using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace UnitySampleAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [System.Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f; // Speed when walking forward
            public float BackwardSpeed = 4.0f; // Speed when walking backwards
            public float StrafeSpeed = 4.0f; // Speed when walking sideways
            public float SprintSpeed = 10.0f; // Speed when sprinting
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;
            private bool running;


            public void UpdateDesiredTargetSpeed()
            {
                if (CrossPlatformInputManager.GetButton("Fire1"))
                {
                    CurrentTargetSpeed = SprintSpeed;
                    running = true;
                    return;
                }
                CurrentTargetSpeed = ForwardSpeed;
                running = false;
            }


            public bool Running
            {
                get { return running; }
            }
        }

        [System.Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character 
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
        }

        public Camera _camera;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        private Rigidbody RigidBody;
        private CapsuleCollider Capsule;
        private float yRotation;
        private Vector3 groundContactNormal;
        private bool jump, previouslyGrounded, jumping, isGrounded;

        public Vector3 Velocity
        {
            get { return RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return isGrounded; }
        }

        public bool Jumping
        {
            get { return jumping; }
        }

        public bool Running
        {
            get { return movementSettings.Running; }
        }


        private void Start()
        {
            RigidBody = GetComponent<Rigidbody>();
            Capsule = GetComponent<CapsuleCollider>();
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !jump)
            {
                jump = true;
            }
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            if ((input.x != 0 || input.y != 0) && (advancedSettings.airControl || isGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = _camera.transform.forward*input.y + _camera.transform.right*input.x;
                desiredMove = (desiredMove - Vector3.Project(desiredMove, groundContactNormal)).normalized;

                desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;
                if (RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                {
                    RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (isGrounded)
            {
                RigidBody.drag = 5f;

                if (jump)
                {
                    RigidBody.drag = 0f;
                    RigidBody.velocity = new Vector3(RigidBody.velocity.x, 0f, RigidBody.velocity.z);
                    RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    jumping = true;
                }

                if (!jumping && input.x == 0f && input.y == 0f && RigidBody.velocity.magnitude < 1f)
                {
                    RigidBody.Sleep();
                }
            }
            else
            {
                RigidBody.drag = 0f;
                if (previouslyGrounded && !jumping)
                {
                    StickToGroundHelper();
                }
            }
            jump = false;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(groundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, Capsule.radius, Vector3.down, out hitInfo,
                                   ((Capsule.height/2f) - Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    RigidBody.velocity = RigidBody.velocity - Vector3.Project(RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            movementSettings.UpdateDesiredTargetSpeed();
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
            return input;
        }


        private void RotateView()
        {
            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;
            Vector2 mouseInput = mouseLook.Clamped(yRotation, transform.localEulerAngles.y);

            // handle the rotation round the x axis on the camera
            _camera.transform.localEulerAngles = new Vector3(-mouseInput.y, _camera.transform.localEulerAngles.y, _camera.transform.localEulerAngles.z);
            yRotation = mouseInput.y;
            transform.localEulerAngles = new Vector3(0, mouseInput.x, 0);

            if (isGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking 
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                RigidBody.velocity = velRotation*RigidBody.velocity;
            }
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            previouslyGrounded = isGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, Capsule.radius, Vector3.down, out hitInfo,
                                   ((Capsule.height/2f) - Capsule.radius) + advancedSettings.groundCheckDistance))
            {
                isGrounded = true;
                groundContactNormal = hitInfo.normal;
            }
            else
            {
                isGrounded = false;
                groundContactNormal = Vector3.up;
            }
            if (!previouslyGrounded && isGrounded && jumping)
            {
                jumping = false;
            }
        }
    }
}