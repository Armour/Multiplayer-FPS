using UnityEngine;
using System.Collections;

public class ShowName : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        GetComponent<NameTag>().enabled = GetComponent<MeshRenderer>().isVisible;
    }

}
