using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EnableDepthBuffer : MonoBehaviour {

    void Awake() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
