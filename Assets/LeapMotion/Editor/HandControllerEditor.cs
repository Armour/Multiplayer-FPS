/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HandController))]
public class HandControllerEditor : Editor {

  private const float BOX_RADIUS = 0.45f;
  private const float BOX_WIDTH = 0.965f;
  private const float BOX_DEPTH = 0.6671f;

  public void OnSceneGUI() {
    HandController controller = (HandController)target;
    Vector3 origin = controller.transform.TransformPoint(Vector3.zero);

    Vector3 local_top_left = new Vector3(-BOX_WIDTH, BOX_RADIUS, BOX_DEPTH);
    Vector3 top_left =
        controller.transform.TransformPoint(BOX_RADIUS * local_top_left.normalized);

    Vector3 local_top_right = new Vector3(BOX_WIDTH, BOX_RADIUS, BOX_DEPTH);
    Vector3 top_right =
        controller.transform.TransformPoint(BOX_RADIUS * local_top_right.normalized);

    Vector3 local_bottom_left = new Vector3(-BOX_WIDTH, BOX_RADIUS, -BOX_DEPTH);
    Vector3 bottom_left =
        controller.transform.TransformPoint(BOX_RADIUS * local_bottom_left.normalized);

    Vector3 local_bottom_right = new Vector3(BOX_WIDTH, BOX_RADIUS, -BOX_DEPTH);
    Vector3 bottom_right =
        controller.transform.TransformPoint(BOX_RADIUS * local_bottom_right.normalized);

    Handles.DrawLine(origin, top_left);
    Handles.DrawLine(origin, top_right);
    Handles.DrawLine(origin, bottom_left);
    Handles.DrawLine(origin, bottom_right);

    Vector3 top_normal = controller.transform.TransformDirection(
        Vector3.Cross(local_top_left, local_top_right));
    float top_angle = Vector3.Angle(local_top_left, local_top_right);
    Handles.DrawWireArc(origin, top_normal,
                        controller.transform.TransformDirection(local_top_left),
                        top_angle, controller.transform.lossyScale.x * BOX_RADIUS);

    Vector3 left_normal = controller.transform.TransformDirection(
        Vector3.Cross(local_bottom_left, local_top_left));
    float left_angle = Vector3.Angle(local_bottom_left, local_top_left);
    Handles.DrawWireArc(origin, left_normal,
                        controller.transform.TransformDirection(local_bottom_left),
                        left_angle, controller.transform.lossyScale.x * BOX_RADIUS);

    Vector3 bottom_normal = controller.transform.TransformDirection(
        Vector3.Cross(local_bottom_left, local_bottom_right));
    float bottom_angle = Vector3.Angle(local_bottom_left, local_bottom_right);
    Handles.DrawWireArc(origin, bottom_normal,
                        controller.transform.TransformDirection(local_bottom_left),
                        bottom_angle, controller.transform.lossyScale.x * BOX_RADIUS);

    Vector3 right_normal = controller.transform.TransformDirection(
        Vector3.Cross(local_bottom_right, local_top_right));
    float right_angle = Vector3.Angle(local_bottom_right, local_top_right);
    Handles.DrawWireArc(origin, right_normal,
                        controller.transform.TransformDirection(local_bottom_right),
                        right_angle, controller.transform.lossyScale.x * BOX_RADIUS);

    Vector3 local_left_face = Vector3.Lerp(local_top_left, local_bottom_left, 0.5f);
    Vector3 local_right_face = Vector3.Lerp(local_top_right, local_bottom_right, 0.5f);

    Vector3 across_normal = controller.transform.TransformDirection(-Vector3.forward);
    float across_angle = Vector3.Angle(local_left_face, local_right_face);
    Handles.DrawWireArc(origin, across_normal,
                        controller.transform.TransformDirection(local_left_face),
                        across_angle, controller.transform.lossyScale.x * BOX_RADIUS);

    Vector3 local_top_face = Vector3.Lerp(local_top_left, local_top_right, 0.5f);
    Vector3 local_bottom_face = Vector3.Lerp(local_bottom_left, local_bottom_right, 0.5f);

    Vector3 depth_normal = controller.transform.TransformDirection(-Vector3.right);
    float depth_angle = Vector3.Angle(local_top_face, local_bottom_face);
    Handles.DrawWireArc(origin, depth_normal,
                        controller.transform.TransformDirection(local_top_face),
                        depth_angle, controller.transform.lossyScale.x * BOX_RADIUS);
  }

  public override void OnInspectorGUI() {
    HandController controller = (HandController)target;

    controller.separateLeftRight = EditorGUILayout.Toggle("Separate Left/Right",
                                                          controller.separateLeftRight);

    if (controller.separateLeftRight) {
      controller.leftGraphicsModel =
          (HandModel)EditorGUILayout.ObjectField("Left Hand Graphics Model",
                                                 controller.leftGraphicsModel,
                                                 typeof(HandModel), true);
      controller.rightGraphicsModel =
          (HandModel)EditorGUILayout.ObjectField("Right Hand Graphics Model",
                                                 controller.rightGraphicsModel,
                                                 typeof(HandModel), true);
      controller.leftPhysicsModel =
          (HandModel)EditorGUILayout.ObjectField("Left Hand Physics Model",
                                                 controller.leftPhysicsModel,
                                                 typeof(HandModel), true);
      controller.rightPhysicsModel =
          (HandModel)EditorGUILayout.ObjectField("Right Hand Physics Model",
                                                 controller.rightPhysicsModel,
                                                 typeof(HandModel), true);
    }
    else {
      controller.leftGraphicsModel = controller.rightGraphicsModel = 
          (HandModel)EditorGUILayout.ObjectField("Hand Graphics Model",
                                                 controller.leftGraphicsModel,
                                                 typeof(HandModel), true);

      controller.leftPhysicsModel = controller.rightPhysicsModel = 
          (HandModel)EditorGUILayout.ObjectField("Hand Physics Model",
                                                 controller.leftPhysicsModel,
                                                 typeof(HandModel), true);
    }

    controller.toolModel = 
        (ToolModel)EditorGUILayout.ObjectField("Tool Model",
                                               controller.toolModel,
                                               typeof(ToolModel), true);
    
    controller.handParent = 
        (Transform)EditorGUILayout.ObjectField("Hand Parent Transform",
                                               controller.handParent,
                                               typeof(Transform), true);

    EditorGUILayout.Space();

    controller.isHeadMounted = EditorGUILayout.Toggle("Is Head Mounted",
                                                      controller.isHeadMounted);

    controller.mirrorZAxis = EditorGUILayout.Toggle("Mirror Z Axis", controller.mirrorZAxis);

    controller.handMovementScale =
        EditorGUILayout.Vector3Field("Hand Movement Scale", controller.handMovementScale);

    controller.destroyHands = EditorGUILayout.Toggle("Destroy Hands",
                                                      controller.destroyHands);

    EditorGUILayout.Space();
    controller.enableRecordPlayback = EditorGUILayout.Toggle("Enable Record/Playback",
                                                             controller.enableRecordPlayback);
    if (controller.enableRecordPlayback) {
      controller.recordingAsset =
          (TextAsset)EditorGUILayout.ObjectField("Recording File",
                                                 controller.recordingAsset,
                                                 typeof(TextAsset), true);
      controller.recorderSpeed = EditorGUILayout.FloatField("Playback Speed Multiplier",
                                                             controller.recorderSpeed);
      controller.recorderLoop = EditorGUILayout.Toggle("Playback Loop",
                                                        controller.recorderLoop);
    }
        
    if (GUI.changed)
      EditorUtility.SetDirty(controller);

    Undo.RecordObject(controller, "Hand Preferences Changed: " + controller.name);
  }
}
