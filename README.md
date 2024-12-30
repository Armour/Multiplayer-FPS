# Multiplayer-FPS

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat)](http://makeapullrequest.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Template from jarvis](https://img.shields.io/badge/Hi-Jarvis-ff69b4.svg)](https://github.com/Armour/Jarvis)

A multiplayer first-person shooter game based on Unity3D. Different types of input devices are supported, including Kinect, Xbox controllers, Leap motion, and VR Glasses. (Each contained in a different branch, *UPDATE: those are not maintained since 2020, use at your own risk*)

## Requirement

[Unity 2022.3.55f1 (LTS)](https://unity.cn/release-notes/lts/2022/2022.3.55f1)

## Game logic and functionality

* Login panel
  * Input your **player name** and the **room name** you want to join
  * Click **'join or create room'** button to join a room or create a new room
  * The network connection state shows on the bottom left corner
    ![img](Images/2.jpg)

* Game interface
  * **Player's HP** on the top left corner
  * The **message panel** on the bottom left corner, which shows status of other players (e.g. dead or respawn)
  * A **gun (AK-47)** is always shown on the bottom right corner in front of every thing you can see
  * A red **shooting sight** is always in the center of the screen
  <img src="Images/3.jpg" style="width:500px"></img>

* Player models
  * All the original models and their animations were found from **[Mixamo](https://www.mixamo.com/)**, which is a pretty good game model website run by Adobe
  * There are three types of player **models**:
    * **Policeman**: a policeman-like model with yellow skin
    * **RobotX**: a robot-like model with dark pink skin
    * **RobotY**: a robot-like model with dark blue skin
    * <img src="Images/9.jpg" height="200px"></img> <img src="Images/11.jpg" height="200px"></img> <img src="Images/10.jpg" height="200px"></img>

  * **Animations**:
    * **Walk** towards four different directions
    * **Run** towards four different directions
    * **Jump** without affecting upper part body (**achieved by unity3d body mask**)
    * **Shoot** without affecting lower part body (**achieved by unity3d body mask**)
    * **Unity Blend Tree**
      * This makes the player walk or run more naturally. It uses interpolation function to map different combinations of user input to different animations.
      * ![img](Images/4.jpg)

  * **State Machine**
    * There are multiple layers in the player state machine.
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
  * **Shooting animation are added** by setting keyframes in unity3d animation panel
  ![img](Images/12.jpg)

* Networking
  * This game uses **Photon Unity Networking 2**, which is a good network model from Unity Assets Store

* Bullet effects
  * Bullets hitting different materials will cause different effects
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
  * Doors will automatically open when there is someone nearby and close when no one is around
  * Before opening
  <img src="Images/18.jpg" style="width:550px"></img>
  * After opening
  <img src="Images/19.jpg" style="width:550px"></img>

## Script files

* **CameraRotation.cs**
  * Rotates the scene camera in every updated frame
* **DoorAnimtion.cs**
  * Controls the door animation and detect if the player enters or exits the door triggering area
* **FpsGun.cs**
  * Controls the gun in first person view, mainly for shooting
* **TpsGun.cs**
  * Controls the gun in third person view (replicated on network), mainly transform and particle effects
* **IKControl.cs**
  * Ensures the model is holding a gun regardless of movements or rotations
* **ImpactLifeCycle.cs**
  * Destroys the bullet object after several seconds to save CPU time and memory
* **NameTag.cs**
  * Displays other players' names above their heads
* **NetworkManager.cs**
  * Controls the whole network connection
* **PlayerHealth.cs**
  * Calculates and updates health points of each player
* **PlayerNetworkMover.cs**
  * Synchronizes the position of the player among different clients

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
  * A more advanced interaction that might become popular in the future
* VR glasses
  * More vivid and closer to reality
  * Recently very popular but devices are most likely expensive
  * Players cannot move now due to the limitation of my device

### Kinect Details

* Tools and Platform:
  * Kinect for Xbox One
  * Kinect for Windows SDK
  * Unity
  * Visual Studio

* Recognition Method:
  * Use Kinect for Windows SKD (BodySourceManager) to get the positions of the player's skeleton. Determine the actions of moving, jumping, shooting based on these positions and regard rotation as an input of the game.

* **Shooting**：
  * Users can trigger shooting by lifting their right arms. The game calculates the distance between the user's right hand and right shoulder based on skeleton nodes. Shooting will be triggered if the calculated distance reaches a threshold.

* **Moving**:
  * Move in the game by stepping forward, backward, leftward, and rightward. The game recognizes moving actions by the offset of right foot’s skeleton node on x-z plane. A movement will be triggered if the offset reaches a threshold.

* **Jumping**:
  * Users can jump in the game. The game calculates offset of right foot’s skeleton node on the z-axis to register a jumping action. Jumping will be triggered if the calculated offset reaches a threshold.

* **View Rotation**:
  * Use your right hand as a virtual mouse to control the camera rotation. The game records the initial position of the left hand as the initial position of the mouse, then calculates the camera rotation by the left hand’s offset.

* <img src="Images/skeleton_overview.png" style="width:110px"> </img><img src="Images/shooting.png" style="width:134px"></img> <img src="Images/jumping.png" style="width:122px"> </img><img src="Images/rotation.png" style="width:156px"></img>

## Contribution

See [CONTRIBUTING.md](https://github.com/Armour/Multiplayer-FPS/blob/master/.github/CONTRIBUTING.md)

## License

[MIT License](https://github.com/Armour/Multiplayer-FPS/blob/master/LICENSE)
