using UnityEngine;
using System.Collections;

public class RecordingControls : MonoBehaviour {

  public KeyCode record = KeyCode.R;
  public KeyCode finishAndSave = KeyCode.S;
  public KeyCode resetRecording = KeyCode.Space;

  void Update () {
    HandController controller = GetComponent<HandController>();
    if (Input.GetKeyDown(record))
      controller.Record();

    if (Input.GetKeyDown(finishAndSave)) {
      Debug.Log("Recording saved to: " + controller.FinishAndSaveRecording());
    }

    if (Input.GetKeyDown(resetRecording))
      controller.ResetRecording();
  }
}
