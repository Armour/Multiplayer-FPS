Leap Motion Core Assets

These assets go along with our V2 Skeletal Tracking. There are multiple
customizable hands, a physical "RigidHand" and some useful gestures that
interact with the physics engine like magnetic pinching. There are also 
prefabs and examples to support Oculus VR Head Mounted Display applications.

Before you begin:
  You first need Leap Motion Skeletal V2 Tracking installed from:
  https://developer.leapmotion.com/

Getting Started:
  You can make the simplest "Hello Hands" scene by dropping the
  HandController.prefab in front of, and below the camera. The HandController
  prefab is a 'virtual' Leap Motion Controller. Since your real hands appear
  above the real Leap Motion Controller, the virtual hands will appear above
  the virtual HandController prefab. You can scale the controller to change
  how big and how far the hands appear from the HandController prefab.

  If you want to get started hitting boxes around a room, the
  ControllerSandBox.prefab might be a good place to start. It is a closed off
  room with walls at the edges of hand tracking. So you should have good
  tracking everywhere in the sandbox.

Physics and the RigidHand prefab:
  Our current physics model is a Box Collider for each bone and palm. This
  is great for patting, touching, flicking, and scooping objects. If you want
  to grab objects you can use some of the resources in the Demos which give 
  you one handed and two handed grabs.

Customizing Hands:
  There are two HandModel fields in the HandController prefab
   - a Hand Graphics Model
   - a Hand Physics Model

  This first object says what the hand should look like, the second says how
  it interacts with the physics engine. You can drag different prefabs into
  these fields to have a different looking hand or change how it interacts
  with the environment.

  The SkeletalHand prefab is a good place to customize your own hand. Just
  put objects into any of the bones on the fingers or in the palm and they'll
  become part of the hand. Keep in mind that you shouldn't have Colliders in
  the Hand Graphics Model because it doesn't get updated on the Physics loop.

Tools:
  If you want to use tools, you can drop a ToolModel object into the
  HandController's ToolModel parameter. There are currently two tools shipped
  with these assets: a flash light and a low fidelity spatula but there will
  be more in the future. Please send in any requests.

Examples:
  There are plenty of examples using these assets available at:
  https://github.com/leapmotion-examples/

Questions/Bugs/Feature Requests?
  Please post on https://community.leapmotion.com/c/development

