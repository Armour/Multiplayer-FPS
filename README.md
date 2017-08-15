# Multiplayer-FPS

A multiplayer first-person shooter game based on Unity3D. To improve players' game experience, I added different kinds of input devices such as Kinect, Xbox controller, Leap motion and VR Glasses. (All in different branches)

## Game logic and function

* Login panel
  * Input your **player name** and the **room name** that you want to join
  * Click **'join or create room'** button to join the room (or create and then join it)
  * The network connection state shows on the bottom left corner
    ![img](Images/2.jpg)

* Game interface
  * **Player's HP** on the top left corner
  * The **message panal** on the bottom left corner, which is used to notify player other players status (like dead or respawn)
  * A **gun (AK-47)** will always show on the bottom right corner in front of every thing you can see
  * A red **shooting sight** always in the center of the screen
  <img src="Images/3.jpg" style="width:500px"></img>

* Player models
  * There are three types of player **models**:
    * **Policeman**: a policeman-like model with yellow skin
    * **RobotX**: a robot-like model with dark pink skin
    * **RobotY**: a robot-like model with dark blue skin
    * <img src="Images/9.jpg" height="200px"></img> <img src="Images/11.jpg" height="200px"></img> <img src="Images/10.jpg" height="200px"></img>

  * **Animations**:
    * **Walk** towards four different directions
    * **Run** towards four different directions
    * **Jump** without affecting upper part body (**achieved by unity3d body mask**)
    * **Shooting** without affecting lower part body (**achieved by unity3d body mask**)
    * All the original models and their animations can be gotten from **[Mixamo](https://www.mixamo.com/)**, which is a pretty good game model website run by Adobe
    * **Unity Blend Tree**
      * This is used to make the player walk or run more naturally. It uses interpolation function to map different combinations of user input to different animations.
      * ![img](Images/4.jpg)

  * **State Machine**
    * Player state machine with many layers
    * <img src="Images/5.jpg" style="width:420px"></img>
    * <img src="Images/6.jpg" style="width:420px"></img>
    * <img src="Images/7.jpg" style="width:420px"></img>
    * <img src="Images/8.jpg" style="width:420px"></img>

* Player movement
  * Walking && Running && Aiming
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594065/02a72084-c429-11e5-84b7-39de1a51d991.jpg" style="width:420px"></img>
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594070/02be2234-c429-11e5-874a-880a710742c1.jpg" style="width:420px"></img>
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594601/c34c19f0-c42b-11e5-9c90-2f2e384030ef.jpg" style="width:420px"></img>
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594069/02b960be-c429-11e5-90b1-49e0ff6be56a.jpg" style="width:420px"></img>
  * Jumping
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594068/02b1568a-c429-11e5-9bbe-cee8760c079b.jpg" style="width:420px"></img>
  * Dying
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594067/02abdd9a-c429-11e5-887f-0c830090ff49.jpg" style="width:420px"></img>
    * <img src="https://cloud.githubusercontent.com/assets/5276065/12594066/02aa6d34-c429-11e5-86ce-ef458bb7f7c3.jpg" style="width:420px"></img>

* Gun model
  * The original gun model (AK-47) was from Unity Assets Store
  * **Added shooting animation** by setting keyframes in unity3d animation panel
  ![img](Images/12.jpg)

* Networking
  * Used **Photon Unity Networking** which is a good network model in Unity Assets Store

* Bullet effects
  * Bullets will have different effects when they hit different materials
    * Wood
    <img src="Images/13.jpg" style="width:510px"></img>
    * Ground
    <img src="Images/14.jpg" style="width:510px"></img>
    * Metal
    <img src="Images/15.jpg" style="width:510px"></img>
    * Concrete
    <img src="Images/16.jpg" style="width:510px"></img>
    * Water
    <img src="Images/17.jpg" style="width:510px"></img>

* Door animation
  * Door will automatically be open when there is someone near it, and close when no one around
  * Before opening
  <img src="Images/18.jpg" style="width:550px"></img>
  * After opening
  <img src="Images/19.jpg" style="width:550px"></img>

## Script files

* **CameraRotation.cs**
  * Used to rotate the scene camera in every updated frame
* **DoorAnimtion.cs**
  * Used to control the door animation and detect if the player enter or exit the door trigger area
* **GunFirstPersonView.cs**
  * Used to control the first person view of gun shooting animation
* **GunShooting.cs**
  * Used to control the gun shooting action on the network domain, sending shooting function to evert client if necessary
* **IKControl.cs**
  * Used to make sure the model holding the gun on their hand no matter how they move or rotate
* **ImpactLifeCycle.cs**
  * Used to destroy the bullet after several seconds to save CPU time and memory
* **NameTag.cs**
  * Used to set other players' name above their head on local game
* **NetworkManager.cs**
  * Used to control the whole network connection
* **PlayerHealth.cs**
  * Used to calculate and update each player's health
* **PlayerNetworkMover.cs**
  * Used to synchronize player's position among different clients
* **ShowName.cs**
  * Used to show the player name above their head
* **WeaponPos.cs**
  * Used to move the gun to the place near the player's hand

### Input Devices

* Mouse and keyboard
  * The traditional way
  * Cheap and easy to use
* Kinect
  * See below for details
  * *This part was implemented by my friend [Ruochen Jiang](https://github.com/VHUCXAONG), many thanks to him!*
* Xbox Controller
  * Like the combination of mouse and keyboard
  * Most Xbox games use this way to play
* Leap Motion
  * User hand gesture to control game
  * More advance, maybe future it will be more popular
* VR glasses
  * More vivid, like reality
  * Recently very popular but devices are most likely expensive
  * Player can't move now due to the limitation of my device

### Kinect Detail

* Tools and Platform:
  * Kinect for Xbox One
  * Kinect for Windows SDK
  * Unity
  * Visual Studio

* Recognition Method:
  * Use Kinect for Windows SKD (BodySourceManager) to get the positions of skeleton of the player, then use those positions to distinguish the actions of moving, jumping, shooting and view rotation as game’s input.

* **Shooting**：
  * Use the action of lifting right arm to shoot in the game. I calculate the distance between the nodes of skeleton of right hand and right shoulder. If it reaches a critical value, then mark the action as shooting.

* **Moving**:
  * Use the action of stepping front, back, left and right to move in the game. I recognize moving actions by the offset of right foot’s skeleton node on x-z plane. If the offset reaches a critical value, it will be recognized as moving.

* **Jumping**:
  * Use the action of jumping to jump in the game. I use offset of right foot’s skeleton node on the z-axis to recognize jumping. If the offset reaches a critical value, I regard the action as jumping.

* **View Rotation**:
  * Use right hand as a virtual mouse to control the camera rotation. I record the initial position of left hand as the initial position of the mouse. Then recognize the camera rotation by left hand’s offset.

* <img src="Images/skeleton_overview.png" style="width:110px"> </img><img src="Images/shooting.png" style="width:134px"></img> <img src="Images/jumping.png" style="width:122px"> </img><img src="Images/rotation.png" style="width:156px"></img>

## License

MIT License
