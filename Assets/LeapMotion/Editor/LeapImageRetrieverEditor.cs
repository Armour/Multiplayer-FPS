using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (LeapImageRetriever))]
public class LeapImageRetrieverEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        SerializedProperty eyeProperty = serializedObject.FindProperty("eye");
        if (eyeProperty.enumValueIndex == -1) {
            LeapImageRetriever retrieverScript = target as LeapImageRetriever;
            bool containsLeft = retrieverScript.gameObject.name.ToLower().Contains("left");
            eyeProperty.enumValueIndex = containsLeft ? (int)LeapImageRetriever.EYE.LEFT : (int)LeapImageRetriever.EYE.RIGHT;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
